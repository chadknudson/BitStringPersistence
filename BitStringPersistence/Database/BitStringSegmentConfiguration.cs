
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BitStringPersistence.Database
{
    public class BitStringSegmentConfiguration : IEntityTypeConfiguration<NorseTechnologies.NorseLibrary.Data.BitString.BitStringSegment>
    {
        public virtual void Configure(EntityTypeBuilder<NorseTechnologies.NorseLibrary.Data.BitString.BitStringSegment> builder)
        {
            builder.ToTable("BitStringSegments");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.BitMask)
                .HasColumnName("BitMask")
                .IsRequired();
        }
    }
}
