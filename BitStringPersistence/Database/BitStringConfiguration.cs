using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NorseTechnologies.NorseLibrary.Data;

namespace BitStringPersistence.Database
{
    public class BitStringConfiguration : IEntityTypeConfiguration<BitString>
    {
        public virtual void Configure(EntityTypeBuilder<BitString> builder)
        {
            builder.ToTable("BitString");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.HasMany(x => x.Segments)
                   .WithOne(e => e.BitString)
                   .HasForeignKey(e => e.BitStringId)
                   .OnDelete(DeleteBehavior.Cascade); // This means that if a BitString is deleted, all its related segments will also be deleted.

            //entity.HasMany(d => d.BitStringSegments).WithMany(p => p.BitStrings)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "BitStringBitStringSegment",
            //        r => r.HasOne<BitStringSegment>().WithMany()
            //            .HasForeignKey("BitStringSegmentId")
            //            .HasConstraintName("FK_dbo.BitStringBitStringSegments_dbo.BitStringSegments_BitStringSegment_Id"),
            //        l => l.HasOne<BitString>().WithMany()
            //            .HasForeignKey("BitStringId")
            //            .HasConstraintName("FK_dbo.BitStringBitStringSegments_dbo.BitString_BitString_Id"),
            //        j =>
            //        {
            //            j.HasKey("BitStringId", "BitStringSegmentId").HasName("PK_dbo.BitStringBitStringSegments");
            //            j.ToTable("BitStringBitStringSegments");
            //            j.HasIndex(new[] { "BitStringSegmentId" }, "IX_BitStringSegment_Id");
            //            j.HasIndex(new[] { "BitStringId" }, "IX_BitString_Id");
            //            j.IndexerProperty<Guid>("BitStringId").HasColumnName("BitString_Id");
            //            j.IndexerProperty<Guid>("BitStringSegmentId").HasColumnName("BitStringSegment_Id");
            //        });

        }
    }
}
