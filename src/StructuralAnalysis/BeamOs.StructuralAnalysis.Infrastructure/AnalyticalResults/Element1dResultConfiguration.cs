using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.Element1dResults;

public class Element1dResultConfiguration : IEntityTypeConfiguration<Element1dResult>
{
    public void Configure(EntityTypeBuilder<Element1dResult> builder)
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
