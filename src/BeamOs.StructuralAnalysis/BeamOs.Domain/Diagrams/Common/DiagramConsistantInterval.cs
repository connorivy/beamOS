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
        Polynomial polynomialDescription,
        DiagramConsistantIntervalId? id = null
    )
        : base(id ?? new())
    {
        this.StartLocation = startLocation;
        this.EndLocation = endLocation;
        this.PolynomialDescription = polynomialDescription;
        this.LengthUnit = startLocation.Unit;
    }

    public Length StartLocation { get; set; }
    public Length EndLocation { get; set; }
    public Polynomial PolynomialDescription { get; set; }
    public LengthUnit LengthUnit { get; set; }

    public double EvalutateAtLocation(Length location)
    {
        if (location < this.StartLocation || location > this.EndLocation)
        {
            throw new Exception("Out of bounds my guy");
        }

        return this.PolynomialDescription.Evaluate(location.As(this.LengthUnit));
    }

    //public bool EqualsIntervalAtLocation(DiagramConsistantInterval other, Length location, Length EqualityTolerance)
    //{
    //    return this.EvalutateAtLocation()
    //}
}
