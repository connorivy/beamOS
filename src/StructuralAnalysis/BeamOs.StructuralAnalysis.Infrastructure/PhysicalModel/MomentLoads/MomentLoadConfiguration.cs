using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.MomentLoads;

public class MomentLoadConfiguration : IEntityTypeConfiguration<MomentLoad>
{
    public void Configure(EntityTypeBuilder<MomentLoad> builder)
    {
        builder
            .HasOne<NodeDefinition>()
            .WithMany(el => el.MomentLoads)
            .HasPrincipalKey(el => new { el.Id, el.ModelId })
            .HasForeignKey(el => new { el.NodeId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<LoadCase>(el => el.LoadCase)
            .WithMany()
            .HasPrincipalKey(el => new { el.Id, el.ModelId })
            .HasForeignKey(el => new { el.LoadCaseId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}
