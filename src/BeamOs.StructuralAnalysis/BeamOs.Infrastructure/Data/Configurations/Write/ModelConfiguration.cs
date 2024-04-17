using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.Infrastructure.Data.Configurations.Write;

internal class ModelConfiguration : IEntityTypeConfiguration<Model>
{
    public void Configure(EntityTypeBuilder<Model> builder)
    {
        _ = builder.HasMany<Node>().WithOne().HasForeignKey(node => node.ModelId).IsRequired();

        _ = builder.HasMany<Element1D>().WithOne().HasForeignKey(el => el.ModelId).IsRequired();
    }
}
