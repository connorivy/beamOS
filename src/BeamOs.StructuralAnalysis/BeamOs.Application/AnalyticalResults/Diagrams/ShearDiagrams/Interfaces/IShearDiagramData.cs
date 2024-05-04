using BeamOs.Application.Common.Interfaces;
using BeamOs.Domain.Diagrams.Common;
using MathNet.Spatial.Euclidean;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Application.AnalyticalResults.Diagrams.ShearDiagrams.Interfaces;

public interface IShearDiagramData : IEntityData
{
    public Guid Element1DId { get; }
    public Vector3D ElementDirection { get; }
    public Vector3D ShearDirection { get; }
    public ForceUnit ForceUnit { get; }
    public Length ElementLength { get; }
    public Length EqualityTolerance { get; }
    public LengthUnit LengthUnit { get; }
    public List<DiagramConsistantInterval>? Intervals { get; }
}
