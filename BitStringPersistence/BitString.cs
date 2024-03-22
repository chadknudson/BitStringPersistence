using System;
using System.Collections.Generic;
using System.Text;

namespace NorseTechnologies.NorseLibrary.Data
{
	public class BitString : IComparable<BitString>
	{
		private Guid id;

		public Guid Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
				foreach (var segment in Segments)
				{
					segment.BitStringId = value;
				}
			}
		}

		public List<BitStringSegment> Segments { get; set; } = new List<BitStringSegment>();
		private int SegmentSize = sizeof(long) * 8;

		public int Capacity { get { return Segments.Count * SegmentSize; } }

		public class BitStringSegment
		{
			public BitStringSegment()
			{ 
			}

			public BitStringSegment(BitString bitstring)
			{
				BitString = bitstring;
				BitStringId = bitstring.Id;
			}

			public Guid Id { get; set; }
			public long BitMask { get; set; }
			public int MaskIndex { get; set; }
			public BitString BitString { get; set; }
			public Guid BitStringId { get; set; }

			public static BitStringSegment Create(BitString bitString, long bitMask, int maskIndex)
			{
				return new BitStringSegment(bitString)
				{
					Id = Guid.NewGuid(),
					BitMask = bitMask,
					MaskIndex = maskIndex,
					BitString = bitString,
					BitStringId = bitString.Id
				};
			}
		}

		public BitString() : this(64)
		{
		}

		public BitString(int bitCount)
		{
			Id = Guid.NewGuid();
			Initialize(bitCount);
		}

		public static BitString Create(int bitCount)
		{
			BitString newBitString = new BitString(bitCount);
			newBitString.Initialize(bitCount);
			return newBitString;
		}

		public BitString Initialize(int bitCount)
		{
			Segments = new List<BitStringSegment>();
			this.Grow(bitCount);
			return this;
		}

		private BitString Grow(int bitCount)
		{
			int existingSegmentCount = Segments.Count;
			int newSegmentCount = (bitCount + (SegmentSize - 1)) / SegmentSize;
			for (int i = 0; i < newSegmentCount; i++)
			{
				Segments.Add(BitStringSegment.Create(this, 0, i));
			}
			return this;
		}

		public BitString SetBit(int bitIndex)
		{
			if (bitIndex < 0)
				throw new ArgumentOutOfRangeException(nameof(bitIndex));

			int maskIndex = bitIndex / SegmentSize;

			if ((maskIndex + 1) > Segments.Count)
			{
				return Grow((maskIndex + 1) * SegmentSize).SetBit(bitIndex);
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

			if ((maskIndex + 1) > Segments.Count)
			{
				return Grow((maskIndex + 1) * SegmentSize).ClearBit(bitIndex);
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

			if ((maskIndex + 1) > Segments.Count)
			{
				return Grow((maskIndex + 1) * SegmentSize).IsBitSet(bitIndex);
			}

			int bitShift = bitIndex % SegmentSize;
			long bitMask = 1L << bitShift;
			return ((Segments[maskIndex].BitMask & bitMask) == bitMask);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			for (int maskIndex = Segments.Count - 1; maskIndex >= 0; maskIndex--)
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

		/// <summary>
		/// Assign the value of another BitString to this BitString.
		/// </summary>
		/// <param name="value"></param>
		/// <returns>this now contains the same values as the value BitString passed in but maintains its original Id and the Ids of the child BitStringSegments.</returns>
		public BitString Assign(BitString value)
		{
			if (value.Segments.Count > this.Segments.Count)
			{
				Grow(value.Segments.Count * SegmentSize);
			}
			for (int i = 0; i < value.Segments.Count; i++)
			{
				Segments[i].BitMask = value.Segments[i].BitMask;
			}
			return this;
		}

		public int CompareTo(BitString other)
		{
			// If other is not a valid object reference, this instance is greater.
			if (other == null) return 1;

			// Simple Case -- the two BitStrings are the same size so we just compare each segment's BitMask with the other
			if (Segments.Count == other.Segments.Count)
			{
				for (int iSegment = Segments.Count - 1; iSegment >= 0; iSegment--)
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
				int minUnitCount = Math.Min(Segments.Count, other.Segments.Count);
				if (Segments.Count > other.Segments.Count)
				{
					for (int iSegment = Segments.Count - 1; iSegment >= minUnitCount; iSegment--)
					{
						if (Segments[iSegment].BitMask > 0)
							return 1;
						if (Segments[iSegment].BitMask < 0)
							return -1;
					}
				}
				else
				{
					for (int iSegment = other.Segments.Count - 1; iSegment >= minUnitCount; iSegment--)
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
			if (operand1.Segments.Count == operand2.Segments.Count)
			{
				for (int iSegment = operand1.Segments.Count - 1; iSegment >= 0; iSegment--)
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
				int minUnitCount = Math.Min(operand1.Segments.Count, operand2.Segments.Count);
				if (operand1.Segments.Count > operand2.Segments.Count)
				{
					for (int iSegment = operand1.Segments.Count - 1; iSegment >= minUnitCount; iSegment--)
					{
						if (operand1.Segments[iSegment].BitMask > 0)
							return false;
						if (operand1.Segments[iSegment].BitMask < 0)
							return false;
					}
				}
				else
				{
					for (int iSegment = operand2.Segments.Count - 1; iSegment >= minUnitCount; iSegment--)
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
			int operatorUnits = Math.Min(value1.Segments.Count, value2.Segments.Count);
			int resultUnits = Math.Max(value1.Segments.Count, value2.Segments.Count);
			BitString results = BitString.Create(resultUnits * value1.SegmentSize);

			for (int i = 0; i < operatorUnits; i++)
			{
				results.Segments[i].BitMask = value1.Segments[i].BitMask & value2.Segments[i].BitMask;
			}
			return results;
		}

		public static BitString operator |(BitString value1, BitString value2)
		{
			int operatorUnits = Math.Min(value1.Segments.Count, value2.Segments.Count);
			int resultUnits = Math.Max(value1.Segments.Count, value2.Segments.Count);
			BitString results = BitString.Create(resultUnits * value1.SegmentSize);

			for (int i = 0; i < operatorUnits; i++)
			{
				results.Segments[i].BitMask = value1.Segments[i].BitMask | value2.Segments[i].BitMask;
			}
			return results;
		}

		/// <summary>
		/// The BitwiseAndAssignment method performs the equivalent of the &= operator but changes the
		/// values only of the left hand operand and does not return a new BitString object. This ensures
		/// that the Id values of the BitString and its child BitStringSegment objects do not change.        /// 
		/// </summary>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		/// <returns>value1 is changed to reflect the result of the bitwise AND with value2.</returns>
		/// <devnote>
		/// C# does not allow the developer to overload the &= operator explicitly. C# does an implicit
		/// overload of the &= operator by assigning the result of the & operation to the left hand 
		/// operand. In our case, this does not work because we have Identity issues where we want to 
		/// maintain the same Id values for the BitString and its child BitStringSegments for persisting
		/// to a database.
		/// </devnote>
		public BitString BitwiseAndAssignment(BitString value)
		{
			int operatorUnits = Math.Min(this.Segments.Count, value.Segments.Count);
			int resultUnits = Math.Max(this.Segments.Count, value.Segments.Count);
			BitString results = BitString.Create(resultUnits * this.SegmentSize);

			for (int i = 0; i < operatorUnits; i++)
			{
				results.Segments[i].BitMask = this.Segments[i].BitMask & value.Segments[i].BitMask;
			}
			return this.Assign(results);
		}

		/// <summary>
		/// The BitwiseOrAssignment method performs the equivalent of the |= operator but changes the
		/// values only of the left hand operand and does not return a new BitString object. This ensures
		/// that the Id values of the BitString and its child BitStringSegment objects do not change.         
		/// </summary>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		/// <returns>value1 is changed to reflect the result of the bitwise OR with value2.</returns>
		/// <devnote>
		/// C# does not allow the developer to overload the |= operator explicitly. C# does an implicit
		/// overload of the &= operator by assigning the result of the | operation to the left hand 
		/// operand. In our case, this does not work because we have Identity issues where we want to 
		/// maintain the same Id values for the BitString and its child BitStringSegments for persisting
		/// to a database.
		/// </devnote>
		public BitString BitwiseOrAssignment(BitString value)
		{
			int operatorUnits = Math.Min(this.Segments.Count, value.Segments.Count);
			int resultUnits = Math.Max(this.Segments.Count, value.Segments.Count);
			BitString results = BitString.Create(resultUnits * this.SegmentSize);

			for (int i = 0; i < operatorUnits; i++)
			{
				results.Segments[i].BitMask = this.Segments[i].BitMask | value.Segments[i].BitMask;
			}
			return this.Assign(results);
		}

		public static BitString operator ^(BitString value1, BitString value2)
		{
			int operatorUnits = Math.Min(value1.Segments.Count, value2.Segments.Count);
			int resultUnits = Math.Max(value1.Segments.Count, value2.Segments.Count);
			BitString results = BitString.Create(resultUnits * value1.SegmentSize);

			for (int i = 0; i < operatorUnits; i++)
			{
				results.Segments[i].BitMask = value1.Segments[i].BitMask ^ value2.Segments[i].BitMask;
			}
			return results;
		}

		public static BitString operator ~(BitString value1)
		{
			BitString results = BitString.Create(value1.Segments.Count * value1.SegmentSize);

			for (int i = 0; i < value1.Segments.Count; i++)
			{
				results.Segments[i].BitMask = ~value1.Segments[i].BitMask;
			}
			return results;
		}

		public static BitString operator <<(BitString value, int shift)
		{
			BitString results = BitString.Create(value.Segments.Count * value.SegmentSize);
			string initialValue = value.ToString();
			string newValue = initialValue.Substring(shift) + new String('0', shift);
			return BitString.ParseBitString(newValue);
		}

		public static BitString operator >>(BitString value, int shift)
		{
			BitString results = BitString.Create(value.Segments.Count * value.SegmentSize);
			string initialValue = value.ToString();
			string leadingDigits = new String(initialValue[0], shift);
			string newValue = leadingDigits + initialValue.Substring(0, initialValue.Length - shift);
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
				for (int i = 0; i < this.Segments.Count; i++)
				{
					hash = hash * 23 + this.Segments[i].BitMask.GetHashCode();
				}
				return hash;
			}
		}
	}
}