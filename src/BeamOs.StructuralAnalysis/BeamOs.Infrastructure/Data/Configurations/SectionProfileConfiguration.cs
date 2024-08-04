using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.Infrastructure.Data.Configurations;

public class SectionProfileConfiguration : IEntityTypeConfiguration<SectionProfile>
{
    public void Configure(EntityTypeBuilder<SectionProfile> builder)
    {
        builder
            .HasMany<Element1D>()
            .WithOne(el => el.SectionProfile)
            .HasForeignKey(el => el.SectionProfileId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}
