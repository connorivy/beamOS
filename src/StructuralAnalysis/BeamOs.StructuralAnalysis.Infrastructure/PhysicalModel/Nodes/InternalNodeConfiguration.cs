using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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

public class NodeDefinitionConfiguration : IEntityTypeConfiguration<NodeDefinition>
{
    public void Configure(EntityTypeBuilder<NodeDefinition> builder)
    {
        builder
            .UseTphMappingStrategy()
            .HasDiscriminator(el => el.NodeType)
            .HasValue<InternalNode>(BeamOsObjectType.InternalNode)
            .HasValue<Node>(BeamOsObjectType.Node);

        builder
            .Property(el => el.NodeType)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);

        // builder
        //     .HasOne(el => el.Model)
        //     .WithMany()
        //     .HasForeignKey(el => el.ModelId)
        //     .IsRequired()
        //     .OnDelete(DeleteBehavior.Cascade);
    }
}
