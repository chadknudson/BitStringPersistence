using Microsoft.EntityFrameworkCore;
using NorseTechnologies.NorseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NorseTechnologies.NorseLibrary.Data.BitString;

namespace BitStringPersistence
{
    public class BitStringDbContext : DbContext
    {
        public DbSet<BitString> BitStrings { get; set; }
        public DbSet<BitStringSegment> BitStringSegments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=BitStringDB;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BitString>(entity =>
            {
                entity.ToTable("BitStrings");
                entity.HasKey(e => e.Id);
                entity.HasMany(e => e.Segments)
                    .WithOne(e => e.BitString)
                    .HasForeignKey(e => e.BitStringId)
                    .IsRequired();
            });

            modelBuilder.Entity<BitStringSegment>(entity =>
            {
                entity.ToTable("BitStringSegments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BitMask)
                    .HasColumnName("BitMask")
                    .IsRequired();
            });
        }
    }

}
