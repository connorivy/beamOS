using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;

public class Element1dResult : BeamOsAnalyticalResultEntity<Element1dId>
{
    public Element1dResult(ModelId modelId, ResultSetId resultSetId, Element1dId element1dId)
        : base(element1dId, resultSetId, modelId) { }

    public required Forces LocalStartForces { get; init; }
    public required Forces LocalEndForces { get; init; }

    public required Displacements LocalStartDisplacements { get; init; }
    public required Displacements LocalEndDisplacements { get; init; }

    public required Force MaxShear { get; init; }
    public required Force MinShear { get; init; }
    public required Torque MaxMoment { get; init; }
    public required Torque MinMoment { get; init; }

    public required Length MaxDisplacement { get; init; }
    public required Length MinDisplacement { get; init; }

    public Element1dId Element1dId => this.Id;

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Element1dResult() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

public readonly record struct DeflectionDiagrams(
    Element1dId Element1dId,
    int NumSteps,
    double[] Offsets
);
