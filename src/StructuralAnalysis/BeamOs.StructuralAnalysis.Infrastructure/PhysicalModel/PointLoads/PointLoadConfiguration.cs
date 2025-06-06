using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.PointLoads;

public class PointLoadConfiguration : IEntityTypeConfiguration<PointLoad>
{
    public void Configure(EntityTypeBuilder<PointLoad> builder)
    {
        builder
            .HasOne(el => el.Model)
            .WithMany(el => el.PointLoads)
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<NodeDefinition>()
            .WithMany(el => el.PointLoads)
            .HasForeignKey(el => new { el.NodeId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        builder
            .HasOne<LoadCase>(el => el.LoadCase)
            .WithMany()
            .HasPrincipalKey(el => new { el.Id, el.ModelId })
            .HasForeignKey(el => new { el.LoadCaseId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}
