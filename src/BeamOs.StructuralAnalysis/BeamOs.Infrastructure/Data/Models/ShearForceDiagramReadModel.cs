using BeamOs.Application.AnalyticalResults.Diagrams.ShearDiagrams.Interfaces;
using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Diagrams.Common;
using MathNet.Spatial.Euclidean;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Infrastructure.Data.Models;

internal class ShearForceDiagramReadModel : BeamOSEntity<Guid>, IShearDiagramData
{
    public Guid Element1DId { get; private set; }
    public Vector3D ElementDirection { get; private set; }
    public Vector3D ShearDirection { get; private set; }
    public ForceUnit ForceUnit { get; }
    public Length ElementLength { get; private set; }
    public Length EqualityTolerance { get; private set; }
    public LengthUnit LengthUnit { get; private set; }
    public List<DiagramConsistantInterval>? Intervals { get; private set; }
}
