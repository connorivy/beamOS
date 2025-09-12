using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.NodeResults;

internal class NodeResultConfiguration : IEntityTypeConfiguration<NodeResult>
{
    public void Configure(EntityTypeBuilder<NodeResult> builder)
    {
        builder
            .HasOne(e => e.ResultSet)
            .WithMany()
            .HasForeignKey(e => e.ResultSetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(e => e.Model)
            .WithMany()
            .HasForeignKey(e => e.ModelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
