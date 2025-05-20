using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Models;

internal class ProposalIssueConfiguration : IEntityTypeConfiguration<ProposalIssue>
{
    public void Configure(EntityTypeBuilder<ProposalIssue> builder)
    {
        _ = builder.HasKey(el => new
        {
            el.Id,
            el.ModelProposalId,
            el.ModelId,
        });

        _ = builder
            .HasIndex(el => new
            {
                el.ModelId,
                el.ModelProposalId,
                el.ExistingId,
                el.ProposedId,
            })
            .IsUnique();

        _ = builder
            .HasOne(m => m.Model)
            .WithMany()
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder
            .HasOne(m => m.ModelProposal)
            .WithMany(m => m.ProposalIssues)
            .HasPrincipalKey(m => new { m.Id, m.ModelId })
            .HasForeignKey(el => new { el.ModelProposalId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
