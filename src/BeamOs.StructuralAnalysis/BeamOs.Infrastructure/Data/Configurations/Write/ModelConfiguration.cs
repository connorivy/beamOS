using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.Infrastructure.Data.Configurations.Write;

internal class ModelConfiguration : IEntityTypeConfiguration<Model>
{
    public void Configure(EntityTypeBuilder<Model> builder)
    {
        _ = builder.HasMany<Node>().WithOne().HasForeignKey(node => node.ModelId).IsRequired();
        _ = builder.HasMany<Element1D>().WithOne().HasForeignKey(el => el.ModelId).IsRequired();
        _ = builder.HasMany<Material>().WithOne().HasForeignKey(el => el.ModelId).IsRequired();
        _ = builder
            .HasMany<SectionProfile>()
            .WithOne()
            .HasForeignKey(el => el.ModelId)
            .IsRequired();

        // these belong to the node
        //_ = builder.HasMany<PointLoad>().WithOne().HasForeignKey(el => el.ModelId).IsRequired();
        //_ = builder.HasMany<MomentLoad>().WithOne().HasForeignKey(el => el.ModelId).IsRequired();
    }
}
