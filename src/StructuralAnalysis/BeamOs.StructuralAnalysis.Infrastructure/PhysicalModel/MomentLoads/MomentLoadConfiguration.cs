using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.MomentLoads;

public class MomentLoadConfiguration : IEntityTypeConfiguration<MomentLoad>
{
    public void Configure(EntityTypeBuilder<MomentLoad> builder)
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
