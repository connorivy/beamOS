using BeamOs.Domain.AnalyticalResults.AnalyticalElement1dAggregate.ValueObjects;
using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;

namespace BeamOs.Domain.AnalyticalResults.Element1dResultAggregate;

public sealed class Element1dResult : AggregateRoot<Element1dResultId>
{
    public Element1dResult(
        Element1DId element1DId,
        ShearForceDiagramId? shearForceDiagramId,
        Element1dResultId? id = null
    )
        : base(id ?? new())
    {
        this.Element1DId = element1DId;
        this.ShearForceDiagramId = shearForceDiagramId;
    }

    public Element1DId Element1DId { get; }
    public ShearForceDiagramId? ShearForceDiagramId { get; }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Element1dResult() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
