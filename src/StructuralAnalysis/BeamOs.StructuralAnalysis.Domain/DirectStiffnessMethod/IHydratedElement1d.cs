using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;

public interface IHydratedElement1d
{
    public Area Area { get; }
    public Element1dId Element1dId { get; }
    public NodeId EndNodeId { get; }
    public Point EndPoint { get; }
    public Length Length { get; }
    public Pressure ModulusOfElasticity { get; }
    public Pressure ModulusOfRigidity { get; }
    public AreaMomentOfInertia PolarMomentOfInertia { get; }
    public Angle SectionProfileRotation { get; }
    public NodeId StartNodeId { get; }
    public Point StartPoint { get; }
    public AreaMomentOfInertia StrongAxisMomentOfInertia { get; }
    public AreaMomentOfInertia WeakAxisMomentOfInertia { get; }
}
