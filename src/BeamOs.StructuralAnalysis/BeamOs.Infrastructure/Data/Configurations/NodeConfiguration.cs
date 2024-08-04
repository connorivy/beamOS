using BeamOs.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.Infrastructure.Data.Configurations;

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
            .HasMany<Element1D>()
            .WithOne(el => el.StartNode)
            .HasForeignKey(el => el.StartNodeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        builder
            .HasMany<Element1D>()
            .WithOne(el => el.EndNode)
            .HasForeignKey(el => el.EndNodeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        builder
            .HasMany(n => n.PointLoads)
            .WithOne()
            .HasForeignKey(el => el.NodeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        builder
            .HasOne(el => el.NodeResult)
            .WithOne()
            .HasForeignKey<NodeResult>(el => el.NodeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // needed for some reason?
        builder.ComplexProperty(n => n.Restraint);
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
