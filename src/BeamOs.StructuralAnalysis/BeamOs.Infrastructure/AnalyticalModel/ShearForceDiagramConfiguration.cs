using BeamOs.Domain.Diagrams.MomentDiagramAggregate;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.Infrastructure.AnalyticalModel;

internal class ShearForceDiagramConfiguration : IEntityTypeConfiguration<ShearForceDiagram>
{
    public void Configure(EntityTypeBuilder<ShearForceDiagram> builder)
    {
        _ = builder.HasMany(el => el.Intervals).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}

internal class MomentForceDiagramConfiguration : IEntityTypeConfiguration<MomentDiagram>
{
    public void Configure(EntityTypeBuilder<MomentDiagram> builder)
    {
        _ = builder.HasMany(el => el.Intervals).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}
