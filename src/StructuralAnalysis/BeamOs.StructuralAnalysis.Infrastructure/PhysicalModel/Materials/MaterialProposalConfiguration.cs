using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Materials;

internal class MaterialProposalConfiguration : IEntityTypeConfiguration<MaterialProposal>
{
    public void Configure(EntityTypeBuilder<MaterialProposal> builder)
    {
        builder
            .HasOne(el => el.Model)
            .WithMany()
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(el => el.ModelProposal)
            .WithMany(el => el.MaterialProposals)
            .HasPrincipalKey(el => new { el.Id, el.ModelId })
            .HasForeignKey(el => new { el.ModelProposalId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.HasKey(n => new { n.Id, n.ModelId });
        builder.Property(n => n.Id).ValueGeneratedNever();
    }
}
