using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common.ValueObjects;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.MomentDiagramAggregate.ValueObjects;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.ShearForceDiagramAggregate.ValueObjects;
using MathNet.Numerics;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common;

internal class DiagramConsistentInterval : BeamOsEntity<DiagramConsistantIntervalId>
{
    public DiagramConsistentInterval(
        Length startLocation,
        Length endLocation,
        Polynomial polynomial,
        DiagramConsistantIntervalId? id = null
    )
        : base(id ?? new())
    {
        this.StartLocation = startLocation;
        this.EndLocation = endLocation;
        this.Polynomial = polynomial;
        this.LengthUnit = startLocation.Unit;
    }

    public Length StartLocation { get; set; }
    public Length EndLocation { get; set; }
    public Polynomial Polynomial { get; set; }
    public LengthUnit LengthUnit { get; set; }

    public double EvaluateAtLocation(Length location)
    {
        if (location < this.StartLocation || location > this.EndLocation)
        {
            throw new Exception("Out of bounds my guy");
        }

        return this.Polynomial.Evaluate(location.As(this.LengthUnit));
    }

    public Length Length => this.EndLocation - this.StartLocation;

    //public bool EqualsIntervalAtLocation(DiagramConsistantInterval other, Length location, Length EqualityTolerance)
    //{
    //    return this.EvalutateAtLocation()
    //}

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private DiagramConsistentInterval() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

internal class ShearDiagramConsistentInterval : DiagramConsistentInterval
{
    public ShearForceDiagramId ShearForceDiagramId { get; set; }

    public ShearDiagramConsistentInterval(
        ShearForceDiagramId shearForceDiagramId,
        Length startLocation,
        Length endLocation,
        Polynomial polynomial,
        DiagramConsistantIntervalId? id = null
    )
        : base(startLocation, endLocation, polynomial, id)
    {
        this.ShearForceDiagramId = shearForceDiagramId;
    }

    public ShearDiagramConsistentInterval(
        ShearForceDiagramId shearForceDiagramId,
        DiagramConsistentInterval diagramConsistentInterval
    )
        : this(
            shearForceDiagramId,
            diagramConsistentInterval.StartLocation,
            diagramConsistentInterval.EndLocation,
            diagramConsistentInterval.Polynomial,
            diagramConsistentInterval.Id
        ) { }

    [Obsolete("EF Core Constructor", true)]
    public ShearDiagramConsistentInterval()
        : base(default, default, default) { }
}

internal class MomentDiagramConsistentInterval : DiagramConsistentInterval
{
    public MomentDiagramId MomentForceDiagramId { get; set; }

    public MomentDiagramConsistentInterval(
        MomentDiagramId momentDiagramId,
        Length startLocation,
        Length endLocation,
        Polynomial polynomial,
        DiagramConsistantIntervalId? id = null
    )
        : base(startLocation, endLocation, polynomial, id)
    {
        this.MomentForceDiagramId = momentDiagramId;
    }

    public MomentDiagramConsistentInterval(
        MomentDiagramId diagramId,
        DiagramConsistentInterval diagramConsistentInterval
    )
        : this(
            diagramId,
            diagramConsistentInterval.StartLocation,
            diagramConsistentInterval.EndLocation,
            diagramConsistentInterval.Polynomial,
            diagramConsistentInterval.Id
        ) { }

    [Obsolete("EF Core Constructor", true)]
    public MomentDiagramConsistentInterval()
        : base(default, default, default) { }
}
