using AngouriMath;
using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Diagrams.Common.ValueObjects;
using MathNet.Numerics;

namespace BeamOs.Domain.Diagrams.Common;

public sealed class DiagramConsistantInterval : BeamOSEntity<DiagramConsistantIntervalId>
{
    public DiagramConsistantInterval(
        double startLocation,
        double endLocation,
        Polynomial polynomialDescription,
        DiagramConsistantIntervalId? id = null
    )
        : base(id ?? new())
    {
        this.StartLocation = startLocation;
        this.EndLocation = endLocation;
        this.PolynomialDescription = polynomialDescription;
    }

    public double StartLocation { get; set; }
    public double EndLocation { get; set; }
    public Polynomial PolynomialDescription { get; set; }
}
