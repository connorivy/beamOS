using BeamOs.Domain.AnalyticalModel.AnalyticalResultsAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.Infrastructure.AnalyticalModel.AnalyticalResultsAggregate;

public class AnalyticalResultsConfiguration : IEntityTypeConfiguration<AnalyticalResults>
{
    public void Configure(EntityTypeBuilder<AnalyticalResults> builder)
    {
        _ = builder
            .HasMany(r => r.NodeResults)
            .WithOne()
            .HasForeignKey(nr => nr.ModelResultId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder
            .HasMany(r => r.ShearForceDiagrams)
            .WithOne()
            .HasForeignKey(el => el.ModelResultId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder
            .HasMany(r => r.MomentDiagrams)
            .WithOne()
            .HasForeignKey(el => el.ModelResultId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
