using Microsoft.EntityFrameworkCore;
using NorseTechnologies.NorseLibrary.Data;
using static NorseTechnologies.NorseLibrary.Data.BitString;

namespace BitStringPersistence.Database
{
    public class BitStringDbContext : DbContext
    {
        public DbSet<BitString> BitStrings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=CHADSDEVLAPTOP\MSSQLSERVER01;Database=BitStringDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //// Specified GUID values for seeding
            //var bitStringGuid1 = new Guid("12345678-1234-1234-1234-123456789001");
            //var bitStringGuid2 = new Guid("12345678-1234-1234-1234-123456789002");
            //var bitStringGuid3 = new Guid("12345678-1234-1234-1234-123456789003");

            //BitString bitString1 = new BitString()
            //{
            //    Id = bitStringGuid1,
            //    Segments = new List<BitStringSegment>()
            //    {
            //        new BitStringSegment { Id = Guid.NewGuid(), MaskIndex = 0, BitMask = 1, BitStringId = bitStringGuid1 },
            //        new BitStringSegment { Id = Guid.NewGuid(), MaskIndex = 1, BitMask = 7, BitStringId = bitStringGuid1 }
            //    }
            //};

            //// Seed BitStrings with specific GUID values
            //modelBuilder.Entity<BitString>().HasData(
            //    new BitString { Id = bitStringGuid1, Segments = new List<BitStringSegment>()
            //        { 
            //            new BitStringSegment { Id = Guid.NewGuid(), MaskIndex = 0, BitMask = 1, BitStringId = bitStringGuid1 },
            //            new BitStringSegment { Id = Guid.NewGuid(), MaskIndex = 1, BitMask = 7, BitStringId = bitStringGuid1 }
            //        }
            //    },
            //    new BitString { Id = bitStringGuid2, Segments = new List<BitStringSegment>()
            //        { new BitStringSegment { Id = Guid.NewGuid(), MaskIndex = 0, BitMask = 2, BitStringId = bitStringGuid2 } }
            //    },
            //    new BitString { Id = bitStringGuid3, Segments = new List<BitStringSegment>()
            //        { new BitStringSegment { Id = Guid.NewGuid(), MaskIndex = 0, BitMask = 3, BitStringId = bitStringGuid3 } }
            //    }
            //);

            // Seed BitStringSegments associated with each BitString
            //modelBuilder.Entity<BitStringSegment>().HasData(
            //    new BitStringSegment { Id = Guid.NewGuid(), BitMask = 1, BitStringId = bitStringGuid1 },
            //    new BitStringSegment { Id = Guid.NewGuid(), BitMask = 2, BitStringId = bitStringGuid2 },
            //    new BitStringSegment { Id = Guid.NewGuid(), BitMask = 3, BitStringId = bitStringGuid3 }
            //);
        }

    }
}
