using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.EnvelopeResultSets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.EnvelopeResultSets;

internal class EnvelopeResultSetConfiguration : IEntityTypeConfiguration<EnvelopeResultSet>
{
    public void Configure(EntityTypeBuilder<EnvelopeResultSet> builder)
    {
        _ = builder
            .HasOne(e => e.Model)
            .WithMany(e => e.EnvelopeResultSets)
            .HasForeignKey(e => e.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
