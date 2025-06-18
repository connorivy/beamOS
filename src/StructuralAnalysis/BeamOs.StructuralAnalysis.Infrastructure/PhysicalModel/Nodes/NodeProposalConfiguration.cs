using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Nodes;

public class NodeProposalConfiguration : IEntityTypeConfiguration<NodeProposal>
{
    public void Configure(EntityTypeBuilder<NodeProposal> builder)
    {
        _ = builder
            .HasOne(el => el.Model)
            .WithMany()
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder
            .HasOne(el => el.ModelProposal)
            .WithMany(el => el.NodeProposals)
            .HasPrincipalKey(el => new { el.Id, el.ModelId })
            .HasForeignKey(el => new { el.ModelProposalId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class InternalNodeProposalConfiguration : IEntityTypeConfiguration<InternalNodeProposal>
{
    public void Configure(EntityTypeBuilder<InternalNodeProposal> builder)
    {
        builder
            .HasOne(el => el.Model)
            .WithMany()
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(el => el.ModelProposal)
            .WithMany(el => el.InternalNodeProposals)
            .HasPrincipalKey(el => new { el.Id, el.ModelId })
            .HasForeignKey(el => new { el.ModelProposalId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
