using System.Text;

namespace NorseTechnologies.NorseLibrary.Data
{
    public class BitString : IComparable<BitString>
    {
        public Guid Id { get; set; }
        public List<BitStringSegment> Segments;
        private int SegmentCount;
        private int SegmentSize = sizeof(long) * 8;

        public int Capacity { get { return SegmentCount * SegmentSize; } }

        public class BitStringSegment
        {
            public Guid Id { get; set; }
            public long BitMask { get; set; }
            public int MaskIndex { get; set; }
            public BitString BitString { get; set; }
            public Guid BitStringId { get; set; }
            public static BitStringSegment Create(long bitMask, int maskIndex)
            {
                return new BitStringSegment()
                {
                    BitMask = bitMask,
                    MaskIndex = maskIndex
                };
            }
        }

        public BitString() : this(64)
        {
        }

        public BitString(int bitCount)
        {
            Initialize(bitCount);
        }

        public static BitString Create(int bitCount)
        {
            BitString newBitString = new BitString(bitCount);
            newBitString.SegmentCount = (bitCount + (newBitString.SegmentSize - 1)) / newBitString.SegmentSize;
            newBitString.Segments = new List<BitStringSegment>();
            for (int i = 0; i < newBitString.SegmentCount; i++)
            {
                newBitString.Segments.Add(BitStringSegment.Create(0, i));
            }
            return newBitString;
        }

        public void Initialize(int bitCount)
        {
            SegmentCount = (bitCount + (SegmentSize - 1)) / SegmentSize;
            Segments = new List<BitStringSegment>();
            for (int i = 0; i < SegmentCount; i++)
            {
                Segments.Add(BitStringSegment.Create(0, i));
            }
        }

        private BitString Grow(int bitCount)
        {
            int newSegmentCount = (bitCount + (SegmentSize - 1)) / SegmentSize;
            List<BitStringSegment> newSegments = new List<BitStringSegment>();
            for (int i = 0; i < newSegmentCount; i++)
            {
                newSegments.Add(BitStringSegment.Create(0, i));
            }
            for (int i = 0; i < SegmentCount; i++)
            {
                newSegments[i] = Segments[i];
            }
            Segments = newSegments;
            SegmentCount = newSegmentCount;
            return this;
        }

        public BitString SetBit(int bitIndex)
        {
            if (bitIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(bitIndex));

            int maskIndex = bitIndex / SegmentSize;

            if ((maskIndex + 1) > SegmentCount)
            {
                BitString grow = Grow((maskIndex + 1) * SegmentSize);
                return grow.SetBit(bitIndex);
            }

            int bitShift = bitIndex % SegmentSize;
            long bitMask = 1L << bitShift;
            Segments[maskIndex].BitMask |= bitMask;
            return this;
        }

        public BitString ClearBit(int bitIndex)
        {
            if (bitIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(bitIndex));

            int maskIndex = bitIndex / SegmentSize;

            if ((maskIndex + 1) > SegmentCount)
            {
                BitString grow = Grow((maskIndex + 1) * SegmentSize);
                return grow.ClearBit(bitIndex);
            }

            int bitShift = bitIndex % SegmentSize;
            long bitMask = 1L << bitShift;
            Segments[maskIndex].BitMask &= 0L;
            return this;
        }

        public bool IsBitSet(int bitIndex)
        {
            if (bitIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(bitIndex));

            int maskIndex = bitIndex / SegmentSize;

            if ((maskIndex + 1) > SegmentCount)
            {
                BitString grow = Grow((maskIndex + 1) * SegmentSize);
                return grow.IsBitSet(bitIndex);
            }

            int bitShift = bitIndex % SegmentSize;
            long bitMask = 1L << bitShift;
            return ((Segments[maskIndex].BitMask & bitMask) == bitMask);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int maskIndex = SegmentCount - 1; maskIndex >= 0; maskIndex--)
            {
                for (int bit = (SegmentSize - 1); bit >= 0; bit--)
                {
                    long bitMask = 1L << bit;
                    if ((Segments[maskIndex].BitMask & bitMask) == bitMask)
                        sb.Append("1");
                    else
                        sb.Append("0");
                }
            }
            return sb.ToString();
        }

        public static BitString ParseBitString(string bitString)
        {
            BitString parsedBitString = Create(bitString.Length);
            int units = (bitString.Length + (parsedBitString.SegmentSize - 1)) / parsedBitString.SegmentSize;
            for (int index = bitString.Length - 1; index >= 0; index--)
            {
                if (bitString.Substring(index, 1) == "1")
                {
                    int bitIndex = (bitString.Length - 1) - index;
                    int maskIndex = bitIndex / parsedBitString.SegmentSize;
                    int bitShift = bitIndex % parsedBitString.SegmentSize;
                    long bitMask = 1L << bitShift;
                    parsedBitString.Segments[maskIndex].BitMask |= bitMask;
                }
            }
            return parsedBitString;
        }

        public int CompareTo(BitString other)
        {
            // If other is not a valid object reference, this instance is greater.
            if (other == null) return 1;

            // Simple Case -- the two BitStrings are the same size so we just compare each segment's BitMask with the other
            if (SegmentCount == other.SegmentCount)
            {
                for (int iSegment = SegmentCount - 1; iSegment >= 0; iSegment--)
                {
                    if (Segments[iSegment].BitMask > other.Segments[iSegment].BitMask)
                        return 1;
                    if (Segments[iSegment].BitMask < other.Segments[iSegment].BitMask)
                        return -1;
                }
            }
            else
            {
                // Compare the values -- extending the comparisons if the bitstrings are of different sizes
                int minUnitCount = Math.Min(SegmentCount, other.SegmentCount);
                if (SegmentCount > other.SegmentCount)
                {
                    for (int iSegment = SegmentCount - 1; iSegment >= minUnitCount; iSegment--)
                    {
                        if (Segments[iSegment].BitMask > 0)
                            return 1;
                        if (Segments[iSegment].BitMask < 0)
                            return -1;
                    }
                }
                else
                {
                    for (int iSegment = other.SegmentCount - 1; iSegment >= minUnitCount; iSegment--)
                    {
                        if (other.Segments[iSegment].BitMask < 0)
                            return 1;
                        if (other.Segments[iSegment].BitMask > 0)
                            return -1;
                    }
                }
                for (int iSegment = minUnitCount - 1; iSegment >= 0; iSegment--)
                {
                    if (Segments[iSegment].BitMask > other.Segments[iSegment].BitMask)
                        return 1;
                    if (Segments[iSegment].BitMask < other.Segments[iSegment].BitMask)
                        return -1;
                }
            }
            return 0;
        }

        // Define the is greater than operator.
        public static bool operator ==(BitString operand1, BitString operand2)
        {
            if (object.ReferenceEquals(null, operand1))
                return object.ReferenceEquals(null, operand2);

            if (object.ReferenceEquals(null, operand2))
                return false;

            // Simple Case -- the two BitStrings are the same size so we just compare each segment's BitMask with the other
            if (operand1.SegmentCount == operand2.SegmentCount)
            {
                for (int iSegment = operand1.SegmentCount - 1; iSegment >= 0; iSegment--)
                {
                    if (operand1.Segments[iSegment].BitMask > operand2.Segments[iSegment].BitMask)
                        return false;
                    if (operand1.Segments[iSegment].BitMask < operand2.Segments[iSegment].BitMask)
                        return false;
                }
            }
            else
            {
                // Compare the values -- extending the comparisons if the bitstrings are of different sizes
                int minUnitCount = Math.Min(operand1.SegmentCount, operand2.SegmentCount);
                if (operand1.SegmentCount > operand2.SegmentCount)
                {
                    for (int iSegment = operand1.SegmentCount - 1; iSegment >= minUnitCount; iSegment--)
                    {
                        if (operand1.Segments[iSegment].BitMask > 0)
                            return false;
                        if (operand1.Segments[iSegment].BitMask < 0)
                            return false;
                    }
                }
                else
                {
                    for (int iSegment = operand2.SegmentCount - 1; iSegment >= minUnitCount; iSegment--)
                    {
                        if (operand2.Segments[iSegment].BitMask < 0)
                            return false;
                        if (operand2.Segments[iSegment].BitMask > 0)
                            return false;
                    }
                }
                for (int iSegment = minUnitCount - 1; iSegment >= 0; iSegment--)
                {
                    if (operand1.Segments[iSegment].BitMask > operand2.Segments[iSegment].BitMask)
                        return false;
                    if (operand1.Segments[iSegment].BitMask < operand2.Segments[iSegment].BitMask)
                        return false;
                }
            }
            return true;
        }

        // Define the is greater than operator.
        public static bool operator !=(BitString operand1, BitString operand2)
        {
            return !(operand1 == operand2);
        }

        // Define the is greater than operator.
        public static bool operator >(BitString operand1, BitString operand2)
        {
            return operand1.CompareTo(operand2) > 0;
        }

        // Define the is less than operator.
        public static bool operator <(BitString operand1, BitString operand2)
        {
            return operand1.CompareTo(operand2) < 0;
        }

        // Define the is greater than or equal to operator.
        public static bool operator >=(BitString operand1, BitString operand2)
        {
            return operand1.CompareTo(operand2) >= 0;
        }

        // Define the is less than or equal to operator.
        public static bool operator <=(BitString operand1, BitString operand2)
        {
            return operand1.CompareTo(operand2) <= 0;
        }

        public static BitString operator &(BitString value1, BitString value2)
        {
            int operatorUnits = Math.Min(value1.SegmentCount, value2.SegmentCount);
            int resultUnits = Math.Max(value1.SegmentCount, value2.SegmentCount);
            BitString results = BitString.Create(resultUnits * value1.SegmentSize);

            for (int i = 0; i < operatorUnits; i++)
            {
                results.Segments[i].BitMask = value1.Segments[i].BitMask & value2.Segments[i].BitMask;
            }
            return results;
        }

        public static BitString operator |(BitString value1, BitString value2)
        {
            int operatorUnits = Math.Min(value1.SegmentCount, value2.SegmentCount);
            int resultUnits = Math.Max(value1.SegmentCount, value2.SegmentCount);
            BitString results = BitString.Create(resultUnits * value1.SegmentSize);

            for (int i = 0; i < operatorUnits; i++)
            {
                results.Segments[i].BitMask = value1.Segments[i].BitMask | value2.Segments[i].BitMask;
            }
            return results;
        }

        public static BitString operator ^(BitString value1, BitString value2)
        {
            int operatorUnits = Math.Min(value1.SegmentCount, value2.SegmentCount);
            int resultUnits = Math.Max(value1.SegmentCount, value2.SegmentCount);
            BitString results = BitString.Create(resultUnits * value1.SegmentSize);

            for (int i = 0; i < operatorUnits; i++)
            {
                results.Segments[i].BitMask = value1.Segments[i].BitMask ^ value2.Segments[i].BitMask;
            }
            return results;
        }

        public static BitString operator ~(BitString value1)
        {
            BitString results = BitString.Create(value1.SegmentCount * value1.SegmentSize);

            for (int i = 0; i < value1.SegmentCount; i++)
            {
                results.Segments[i].BitMask = ~value1.Segments[i].BitMask;
            }
            return results;
        }

        public static BitString operator <<(BitString value, int shift)
        {
            BitString results = BitString.Create(value.SegmentCount * value.SegmentSize);
            string initialValue = value.ToString();
            string newValue = initialValue.Substring(shift) + new String('0', shift);
            return BitString.ParseBitString(newValue);
        }

        public static BitString operator >>(BitString value, int shift)
        {
            BitString results = BitString.Create(value.SegmentCount * value.SegmentSize);
            string initialValue = value.ToString();
            string leadindDigits = new String(initialValue[0], shift);
            string newValue = leadindDigits + initialValue.Substring(0, initialValue.Length - shift);
            return BitString.ParseBitString(newValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this == (BitString)obj;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                for (int i = 0; i < this.SegmentCount; i++)
                {
                    hash = hash * 23 + this.Segments[i].BitMask.GetHashCode();
                }
                return hash;
            }
        }
    }
}