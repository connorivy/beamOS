using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.Infrastructure.Data.Configurations;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder
            .HasMany<Element1D>()
            .WithOne(el => el.Material)
            .HasForeignKey(el => el.MaterialId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}
