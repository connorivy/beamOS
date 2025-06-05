using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Nodes;

public class InternalNodeConfiguration : IEntityTypeConfiguration<InternalNode>
{
    public void Configure(EntityTypeBuilder<InternalNode> builder)
    {
        builder
            .HasOne(el => el.Model)
            .WithMany(el => el.InternalNodes)
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // builder
        //     .HasMany(n => n.PointLoads)
        //     .WithOne()
        //     .HasPrincipalKey(el => new { el.Id, el.ModelId })
        //     .HasForeignKey(el => new { el.NodeId, el.ModelId })
        //     .IsRequired()
        //     .OnDelete(DeleteBehavior.Cascade);

        // builder
        //     .HasMany(n => n.MomentLoads)
        //     .WithOne()
        //     .HasPrincipalKey(el => new { el.Id, el.ModelId })
        //     .HasForeignKey(el => new { el.NodeId, el.ModelId })
        //     .IsRequired()
        //     .OnDelete(DeleteBehavior.Cascade);
    }
}
