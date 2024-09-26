using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.Infrastructure.Data.Configurations;

public class Element1dConfiguration : IEntityTypeConfiguration<Element1D>
{
    public void Configure(EntityTypeBuilder<Element1D> builder)
    {
        _ = builder
            .HasOne(el => el.Material)
            .WithMany()
            .HasForeignKey(el => el.MaterialId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        _ = builder
            .HasOne(el => el.SectionProfile)
            .WithMany()
            .HasForeignKey(el => el.SectionProfileId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        _ = builder
            .HasMany(el => el.ShearForceDiagrams)
            .WithOne(el => el.Element1d)
            .HasForeignKey(el => el.Element1DId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        _ = builder
            .HasMany(el => el.MomentDiagrams)
            .WithOne(el => el.Element1d)
            .HasForeignKey(el => el.Element1DId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}
