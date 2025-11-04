using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Models;

internal class OctreeConfiguration : IEntityTypeConfiguration<Octree>
{
    public void Configure(EntityTypeBuilder<Octree> builder)
    {
        builder
            .HasOne<Model>()
            .WithOne(m => m.NodeOctree)
            .HasForeignKey<Octree>(o => o.ModelId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property(o => o.Id).HasConversion<OctreeIdConverter>();
        builder.Property(o => o.ModelId).HasConversion<ModelIdConverter>();

        builder.OwnsOne(
            o => o.Root,
            navigationBuilder =>
            {
                navigationBuilder.Property(r => r.Id).HasConversion<OctreeNodeIdConverter>();
                navigationBuilder.Ignore(r => r.Objects);
                navigationBuilder.Ignore(r => r.Children);
            }
        );
    }
}
