using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.LoadCases;

public class LoadCaseConfiguration : IEntityTypeConfiguration<LoadCase>
{
    public void Configure(EntityTypeBuilder<LoadCase> builder)
    {
        _ = builder
            .HasOne(el => el.Model)
            .WithMany(el => el.LoadCases)
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
