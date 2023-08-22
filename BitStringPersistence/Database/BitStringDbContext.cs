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
    }
}
