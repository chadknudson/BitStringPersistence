using NorseTechnologies.NorseLibrary.Data;

namespace BitStringPersistence
{
    public static class Extensions
    {
        public static void DebugDump(this BitString bitString)
        {
            Console.WriteLine($"BitString: Id={bitString.Id}");
            Console.WriteLine("BitString Value:" + bitString.ToString());
            int iSegment = 0;
            foreach (var segment in bitString.Segments)
            {
                Console.WriteLine($"Segment[{iSegment++}]: Id={segment.Id}, BitStringId={segment.BitStringId}, MaskIndex={segment.MaskIndex}, Value={segment.BitMask}");
            }
        }
    }
}
