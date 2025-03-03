using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.ResultSets;

public class ResultSetConfiguration : IEntityTypeConfiguration<ResultSet>
{
    public void Configure(EntityTypeBuilder<ResultSet> builder)
    {
        _ = builder
            .HasOne(e => e.Model)
            .WithMany(e => e.ResultSets)
            .HasForeignKey(e => e.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder
            .HasMany(el => el.NodeResults)
            .WithOne(el => el.ResultSet)
            .HasPrincipalKey(el => new { el.Id, el.ModelId })
            .HasForeignKey(el => new { el.ResultSetId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder
            .HasMany(el => el.Element1dResults)
            .WithOne(el => el.ResultSet)
            .HasPrincipalKey(el => new { el.Id, el.ModelId })
            .HasForeignKey(el => new { el.ResultSetId, el.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        //_ = builder
        //    .HasMany(el => el.ShearForceDiagrams)
        //    .WithOne(el => el.ResultSet)
        //    .HasForeignKey(el => el.ResultSetId)
        //    .IsRequired()
        //    .OnDelete(DeleteBehavior.ClientCascade);

        //_ = builder
        //    .HasMany(el => el.MomentDiagrams)
        //    .WithOne(el => el.ResultSet)
        //    .HasForeignKey(el => el.ResultSetId)
        //    .IsRequired()
        //    .OnDelete(DeleteBehavior.ClientCascade);
    }
}
