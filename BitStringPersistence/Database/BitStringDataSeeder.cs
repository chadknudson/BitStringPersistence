using NorseTechnologies.NorseLibrary.Data;
using static NorseTechnologies.NorseLibrary.Data.BitString;

namespace BitStringPersistence.Database
{
    public static class BitStringDataSeeder
    {
        public static void SeedData(BitStringDbContext context)
        {
            // Call static methods of this class to seed data...
            SeedBitStrings(context);
            context.SaveChanges();
        }

        public static void SeedBitStrings(BitStringDbContext context)
        {
            // Specified GUID values for seeding
            Guid bitStringGuid1 = new Guid("12345678-1234-1234-1234-123456789001");
            Guid bitStringGuid2 = new Guid("12345678-1234-1234-1234-123456789002");
            Guid bitStringGuid3 = new Guid("12345678-1234-1234-1234-123456789003");

            Guid segment1 = new Guid("12345678-1234-1234-1234-100000000001");
            Guid segment2 = new Guid("12345678-1234-1234-1234-100000000002");
            Guid segment3 = new Guid("12345678-1234-1234-1234-200000000001");
            Guid segment4 = new Guid("12345678-1234-1234-1234-300000000001");

            BitString bitString1 = new BitString(128);
            bitString1.SetBit(0);
            bitString1.SetBit(64);
            bitString1.SetBit(65);
            bitString1.SetBit(66);
            bitString1.Id = bitStringGuid1;
            bitString1.Segments[0].Id = segment1;
            bitString1.Segments[1].Id = segment2;
            foreach(var segment in bitString1.Segments) 
            {
                segment.BitStringId = bitStringGuid1;
            }

            BitString bitString2 = new BitString(64);
            bitString2.SetBit(0);
            bitString2.SetBit(1);
            bitString2.SetBit(2);
            bitString2.SetBit(3);
            bitString2.SetBit(4);
            bitString2.SetBit(5);
            bitString2.SetBit(6);
            bitString2.SetBit(7);
            bitString2.Id = bitStringGuid2;
            bitString2.Segments[0].Id = segment3;
            bitString2.Segments[0].BitStringId = bitStringGuid2;

            BitString bitString3 = new BitString(64);
            bitString3.SetBit(0);
            bitString3.SetBit(1);
            bitString3.SetBit(2);
            bitString3.SetBit(3);
            bitString3.SetBit(4);
            bitString3.SetBit(5);
            bitString3.SetBit(6);
            bitString3.SetBit(7);
            bitString3.Id = bitStringGuid3;
            bitString3.Segments[0].Id = segment4;
            bitString3.Segments[0].BitStringId = bitStringGuid3;

            context.Add(bitString1);
            context.Add(bitString2);
            context.Add(bitString3);

            context.SaveChanges();
        }
    }
}

