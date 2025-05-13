using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.Element1dResults;

public class Element1dResultConfiguration : IEntityTypeConfiguration<Element1dResult>
{
    public void Configure(EntityTypeBuilder<Element1dResult> builder)
    {
        builder
            .HasOne(e => e.ResultSet)
            .WithMany()
            .HasPrincipalKey(el => new { el.Id, el.ModelId })
            .HasForeignKey(el => new { el.ResultSetId, el.ModelId })
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(e => e.Model)
            .WithMany()
            .HasForeignKey(e => e.ModelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
