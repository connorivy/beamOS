using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Domain.Models;
using BeamOs.Domain.AnalyticalModel.AnalyticalResultsAggregate.ValueObjects;
using BeamOs.Domain.AnalyticalModel.Element1dResultAggregate;
using BeamOs.Domain.AnalyticalModel.NodeResultAggregate;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using UnitsNet;

namespace BeamOs.Domain.AnalyticalModel.AnalyticalResultsAggregate;

public class AnalyticalResults : AggregateRoot<AnalyticalResultsId>
{
    public AnalyticalResults(ModelId modelId, AnalyticalResultsId? id = null)
        : base(id ?? new())
    {
        this.ModelId = modelId;
    }

    [SetsRequiredMembers]
    public AnalyticalResults(
        ModelId modelId,
        Force maxShear,
        Force minShear,
        Torque maxMoment,
        Torque minMoment,
        AnalyticalResultsId? id = null
    )
        : this(modelId, id)
    {
        this.MaxShear = maxShear;
        this.MinShear = minShear;
        this.MaxMoment = maxMoment;
        this.MinMoment = minMoment;
    }

    public ModelId ModelId { get; private set; }
    public required Force MaxShear { get; init; }
    public required Force MinShear { get; init; }
    public required Torque MaxMoment { get; init; }
    public required Torque MinMoment { get; init; }
    public ICollection<NodeResult>? NodeResults { get; init; }
    public ICollection<Element1dResult>? Element1dResults { get; init; }
    public ICollection<ShearForceDiagram>? ShearForceDiagrams { get; init; }
    public ICollection<MomentDiagram>? MomentDiagrams { get; init; }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private AnalyticalResults() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
