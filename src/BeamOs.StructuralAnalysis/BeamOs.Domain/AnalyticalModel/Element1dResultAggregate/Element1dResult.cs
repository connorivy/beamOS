using BeamOs.Common.Domain.Models;
using BeamOs.Domain.AnalyticalModel.AnalyticalResultsAggregate.ValueObjects;
using BeamOs.Domain.AnalyticalModel.Element1dResultAggregate.ValueObjects;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;

namespace BeamOs.Domain.AnalyticalModel.Element1dResultAggregate;

public sealed class Element1dResult : AggregateRoot<Element1dResultId>
{
    public Element1dResult(
        AnalyticalResultsId modelResultId,
        Element1DId element1DId,
        ShearForceDiagramId? shearForceDiagramId,
        Element1dResultId? id = null
    )
        : base(id ?? new())
    {
        this.ModelResultId = modelResultId;
        this.Element1DId = element1DId;
        this.ShearForceDiagramId = shearForceDiagramId;
    }

    public AnalyticalResultsId ModelResultId { get; private set; }
    public Element1DId Element1DId { get; private set; }
    public ShearForceDiagramId? ShearForceDiagramId { get; private set; }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Element1dResult() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
