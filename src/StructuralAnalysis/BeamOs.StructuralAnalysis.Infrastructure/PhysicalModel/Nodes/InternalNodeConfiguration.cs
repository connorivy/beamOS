using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Nodes;

internal class InternalNodeConfiguration : IEntityTypeConfiguration<InternalNode>
{
    public void Configure(EntityTypeBuilder<InternalNode> builder)
    {
        builder
            .HasOne(el => el.Model)
            .WithMany(el => el.InternalNodes)
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal class NodeDefinitionConfiguration : IEntityTypeConfiguration<NodeDefinition>
{
    public void Configure(EntityTypeBuilder<NodeDefinition> builder)
    {
        builder.HasKey(n => new { n.Id, n.ModelId });
        builder.Property(n => n.Id).ValueGeneratedNever();

        builder
            .UseTphMappingStrategy()
            .HasDiscriminator(el => el.NodeType)
            .HasValue<InternalNode>(BeamOsObjectType.InternalNode)
            .HasValue<Node>(BeamOsObjectType.Node);

        builder
            .Property(el => el.NodeType)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
    }
}
