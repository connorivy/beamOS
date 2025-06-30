using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Nodes;

public class NodeConfiguration : IEntityTypeConfiguration<Node>
{
    public void Configure(EntityTypeBuilder<Node> builder)
    {
        builder
            .HasOne(el => el.Model)
            .WithMany(el => el.Nodes)
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // builder
        //     .HasMany(n => n.PointLoads)
        //     .WithOne()
        //     .HasForeignKey(el => new { el.NodeId, el.ModelId })
        //     .IsRequired()
        //     .OnDelete(DeleteBehavior.ClientCascade);

        // builder
        //     .HasMany(n => n.MomentLoads)
        //     .WithOne()
        //     .HasForeignKey(el => new { el.NodeId, el.ModelId })
        //     .IsRequired()
        //     .OnDelete(DeleteBehavior.ClientCascade);
    }
}
