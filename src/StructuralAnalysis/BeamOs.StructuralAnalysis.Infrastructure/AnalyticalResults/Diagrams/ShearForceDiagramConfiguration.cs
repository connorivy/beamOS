using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.ShearForceDiagramAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.Diagrams;

public class ShearForceDiagramConfiguration : IEntityTypeConfiguration<ShearForceDiagram>
{
    public void Configure(EntityTypeBuilder<ShearForceDiagram> builder)
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
