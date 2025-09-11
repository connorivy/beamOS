using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.SectionProfiles;

internal class SectionProfileProposalFromLibraryConfiguration
    : IEntityTypeConfiguration<SectionProfileProposalFromLibrary>
{
    public void Configure(EntityTypeBuilder<SectionProfileProposalFromLibrary> builder)
    {
        builder
            .HasOne(el => el.Model)
            .WithMany()
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(el => el.ModelProposal)
            .WithMany(el => el.SectionProfileProposalsFromLibrary)
            .HasPrincipalKey(el => new { el.Id, el.ModelId })
            .HasForeignKey(el => new { el.ModelProposalId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
