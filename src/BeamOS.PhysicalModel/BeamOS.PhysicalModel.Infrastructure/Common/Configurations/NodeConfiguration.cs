using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOS.PhysicalModel.Infrastructure.Common.Configurations;

public class NodeConfiguration : IEntityTypeConfiguration<Node>
{
    public void Configure(EntityTypeBuilder<Node> builder)
    {
        builder.HasMany<PointLoad>().WithOne().HasForeignKey(pl => pl.NodeId).IsRequired();

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
