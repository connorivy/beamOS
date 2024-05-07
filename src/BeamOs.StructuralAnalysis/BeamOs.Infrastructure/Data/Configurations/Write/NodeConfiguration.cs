using BeamOs.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.Infrastructure.Data.Configurations.Write;

public class NodeConfiguration : IEntityTypeConfiguration<Node>
{
    public void Configure(EntityTypeBuilder<Node> builder)
    {
        builder.HasMany<PointLoad>().WithOne().HasForeignKey(el => el.NodeId).IsRequired();
        builder.HasMany<MomentLoad>().WithOne().HasForeignKey(el => el.NodeId).IsRequired();
        builder
            .HasOne<NodeResult>()
            .WithOne()
            .HasForeignKey<NodeResult>(el => el.NodeId)
            .IsRequired();

        // needed for some reason?
        builder.ComplexProperty(n => n.Restraint);
    }
}

//public class PointLoadConfiguration : IEntityTypeConfiguration<PointLoad>
//{
//    public void Configure(EntityTypeBuilder<PointLoad> builder)
//    {
//        builder
//            .HasOne<Node>()
//            .WithMany()
//            .HasForeignKey(node => pl.PointLoadId)
//            .IsRequired();
//    }
//}
