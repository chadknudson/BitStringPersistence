
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BitStringPersistence.Database
{
    public class BitStringSegmentConfiguration : IEntityTypeConfiguration<NorseTechnologies.NorseLibrary.Data.BitString.BitStringSegment>
    {
        public virtual void Configure(EntityTypeBuilder<NorseTechnologies.NorseLibrary.Data.BitString.BitStringSegment> builder)
        {
            builder.ToTable("BitStringSegments");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.BitMask)
                .HasColumnName("BitMask")
                .IsRequired();

            builder.HasOne(s => s.BitString)
                   .WithMany(bs => bs.Segments)
                   .HasForeignKey(s => s.BitStringId);
        }
    }
}
