using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Element1ds;

internal class Element1dConfiguration : IEntityTypeConfiguration<Element1d>
{
    public void Configure(EntityTypeBuilder<Element1d> builder)
    {
        builder.HasKey(n => new { n.Id, n.ModelId });
        builder.Property(n => n.Id).ValueGeneratedNever();

        _ = builder
            .HasOne(el => el.StartNode)
            .WithMany(el => el.StartNodeElements)
            .HasForeignKey(el => new { el.StartNodeId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder
            .HasOne(el => el.EndNode)
            .WithMany(el => el.EndNodeElements)
            .HasForeignKey(el => new { el.EndNodeId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder
            .HasOne(el => el.Material)
            .WithMany()
            .HasForeignKey(el => new { el.MaterialId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        _ = builder
            .HasOne(el => el.SectionProfile)
            .WithMany()
            .HasForeignKey(el => new { el.SectionProfileId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        _ = builder
            .HasMany(el => el.InternalNodes)
            .WithOne(el => el.Element1d)
            .HasPrincipalKey(el => new { el.Id, el.ModelId })
            .HasForeignKey(el => new { el.Element1dId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
