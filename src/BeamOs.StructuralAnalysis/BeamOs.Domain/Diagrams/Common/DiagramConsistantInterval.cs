using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Diagrams.Common.ValueObjects;
using MathNet.Numerics;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.Diagrams.Common;

public sealed class DiagramConsistantInterval : BeamOSEntity<DiagramConsistantIntervalId>
{
    public DiagramConsistantInterval(
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

    public double EvalutateAtLocation(Length location)
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
    private DiagramConsistantInterval() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
