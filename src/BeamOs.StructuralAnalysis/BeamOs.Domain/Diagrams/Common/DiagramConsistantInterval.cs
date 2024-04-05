using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Diagrams.Common.ValueObjects;
using MathNet.Numerics;
using UnitsNet;

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
    }

    public Length StartLocation { get; set; }
    public Length EndLocation { get; set; }
    public Polynomial PolynomialDescription { get; set; }
}
