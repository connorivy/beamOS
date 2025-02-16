using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.Element1dResults;

public class Element1dResultConfiguration : IEntityTypeConfiguration<Element1dResult>
{
    public void Configure(EntityTypeBuilder<Element1dResult> builder)
    {
#if SQL_SERVER
        builder
            .HasOne(e => e.Model)
            .WithMany()
            .HasForeignKey(e => e.ModelId)
            .OnDelete(DeleteBehavior.Restrict);
#endif
    }
}
