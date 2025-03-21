using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Nodes;

public class NodeConfiguration : IEntityTypeConfiguration<Node>
{
    public void Configure(EntityTypeBuilder<Node> builder)
    {
        //builder.HasMany<PointLoad>().WithOne().HasForeignKey(el => el.NodeId).IsRequired();
        //builder.HasMany<MomentLoad>().WithOne().HasForeignKey(el => el.NodeId).IsRequired();
        //builder
        //    .HasOne<NodeResult>()
        //    .WithOne()
        //    .HasForeignKey<NodeResult>(el => el.NodeId)
        //    .IsRequired();

        builder
            .HasMany<Element1d>()
            .WithOne(el => el.StartNode)
            .HasForeignKey(el => new { el.StartNodeId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        builder
            .HasMany<Element1d>()
            .WithOne(el => el.EndNode)
            .HasForeignKey(el => new { el.EndNodeId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        builder
            .HasMany(n => n.PointLoads)
            .WithOne()
            .HasForeignKey(el => new { el.NodeId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        builder
            .HasMany(n => n.MomentLoads)
            .WithOne()
            .HasForeignKey(el => new { el.NodeId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        //builder
        //    .HasMany(n => n.MomentLoads)
        //    .WithOne()
        //    .HasForeignKey(el => el.NodeId)
        //    .IsRequired()
        //    .OnDelete(DeleteBehavior.ClientCascade);

        //builder
        //    .HasOne(el => el.NodeResult)
        //    .WithOne()
        //    .HasForeignKey<NodeResult>(el => el.NodeId)
        //    .IsRequired()
        //    .OnDelete(DeleteBehavior.ClientCascade);

        // needed for some reason?
        //builder.ComplexProperty(n => n.Restraint);
    }
}

//public class NodeResultConfiguration : IEntityTypeConfiguration<NodeResult>
//{
//    public void Configure(EntityTypeBuilder<NodeResult> builder)
//    {
//        //builder.HasMany<PointLoad>().WithOne().HasForeignKey(el => el.NodeId).IsRequired();
//        //builder.HasMany<MomentLoad>().WithOne().HasForeignKey(el => el.NodeId).IsRequired();
//        //builder
//        //    .HasOne<NodeResult>()
//        //    .WithOne()
//        //    .HasForeignKey<NodeResult>(el => el.NodeId)
//        //    .IsRequired();

//        builder
//            .HasOne<Node>()
//            .WithOne(n => n.NodeResult)
//            .HasForeignKey(n => n.N)

//        // needed for some reason?
//        builder.ComplexProperty(n => n.Restraint);
//    }
//}
