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
                   .IsRequired();

        }
    }
}
