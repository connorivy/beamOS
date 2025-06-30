using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Models;

internal class ModelProposalConfiguration : IEntityTypeConfiguration<ModelProposal>
{
    public void Configure(EntityTypeBuilder<ModelProposal> builder)
    {
        _ = builder
            .HasOne(m => m.Model)
            .WithMany(el => el.ModelProposals)
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
