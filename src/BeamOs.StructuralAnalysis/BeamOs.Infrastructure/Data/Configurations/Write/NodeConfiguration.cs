using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.Infrastructure.Data.Configurations.Write;

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

        // needed for some reason?
        builder.ComplexProperty(n => n.Restraint);
    }
}
