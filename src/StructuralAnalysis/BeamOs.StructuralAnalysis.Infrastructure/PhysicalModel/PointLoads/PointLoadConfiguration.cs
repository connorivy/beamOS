using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.PointLoads;

public class PointLoadConfiguration : IEntityTypeConfiguration<PointLoad>
{
    public void Configure(EntityTypeBuilder<PointLoad> builder)
    {
        builder
            .HasOne<LoadCase>(el => el.LoadCase)
            .WithMany()
            .HasPrincipalKey(el => new { el.Id, el.ModelId })
            .HasForeignKey(el => new { el.LoadCaseId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}
