using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.NodeResults;

public class NodeResultConfiguration : IEntityTypeConfiguration<NodeResult>
{
    public void Configure(EntityTypeBuilder<NodeResult> builder)
    {
        builder.HasKey(
            el =>
                new
                {
                    el.Id,
                    el.ResultSetId,
                    el.ModelId
                }
        );
        //builder.ComplexProperty(n => n.Displacements);

#if SQL_SERVER
        builder
            .HasOne(e => e.Model)
            .WithMany()
            .HasForeignKey(e => e.ModelId)
            .OnDelete(DeleteBehavior.Restrict);
#endif
    }
}
