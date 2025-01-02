using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Element1ds;

public class Element1dConfiguration : IEntityTypeConfiguration<Element1d>
{
    public void Configure(EntityTypeBuilder<Element1d> builder)
    {
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

        //_ = builder
        //    .HasMany(el => el.ShearForceDiagrams)
        //    .WithOne(el => el.Element1d)
        //    .HasForeignKey(el => el.Element1DId)
        //    .IsRequired()
        //    .OnDelete(DeleteBehavior.ClientCascade);

        //_ = builder
        //    .HasMany(el => el.MomentDiagrams)
        //    .WithOne(el => el.Element1d)
        //    .HasForeignKey(el => el.Element1DId)
        //    .IsRequired()
        //    .OnDelete(DeleteBehavior.ClientCascade);
    }
}
