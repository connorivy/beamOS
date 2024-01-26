using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOS.PhysicalModel.Infrastructure.Common.Configurations;

internal class ModelConfiguration : IEntityTypeConfiguration<Model>
{
    public void Configure(EntityTypeBuilder<Model> builder)
    {
        builder.HasMany<Node>().WithOne().HasForeignKey(node => node.ModelId).IsRequired();

        builder.HasMany<Element1D>().WithOne().HasForeignKey(el => el.ModelId).IsRequired();
    }
}
