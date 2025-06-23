using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Element1ds;

public class Element1dProposalConfiguration : IEntityTypeConfiguration<Element1dProposal>
{
    public void Configure(EntityTypeBuilder<Element1dProposal> builder)
    {
        _ = builder.HasKey(el => new
        {
            el.Id,
            el.ModelProposalId,
            el.ModelId,
        });

        _ = builder
            .HasOne(el => el.Model)
            .WithMany()
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder
            .HasOne(el => el.ModelProposal)
            .WithMany(el => el.Element1dProposals)
            .HasPrincipalKey(el => new { el.Id, el.ModelId })
            .HasForeignKey(el => new { el.ModelProposalId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
