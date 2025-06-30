using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.EnvelopeResultSets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.EnvelopeResultSets;

public class EnvelopeElement1dResultConfiguration
    : IEntityTypeConfiguration<EnvelopeElement1dResult>
{
    public void Configure(EntityTypeBuilder<EnvelopeElement1dResult> builder)
    {
        _ = builder
            .HasOne(e => e.Model)
            .WithMany()
            .HasForeignKey(e => e.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(e => e.EnvelopeResultSet)
            .WithMany(e => e.Element1dResults)
            .HasPrincipalKey(el => new { el.Id, el.ModelId })
            .HasForeignKey(el => new { el.EnvelopeResultSetId, el.ModelId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
