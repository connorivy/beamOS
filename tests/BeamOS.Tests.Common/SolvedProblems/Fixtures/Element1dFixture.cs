using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using UnitsNet;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

public class Element1dFixture(
    NodeFixture startNode,
    NodeFixture endNode,
    MaterialFixture material,
    SectionProfileFixture sectionProfile
) : FixtureBase
{
    public NodeFixture StartNode { get; } = startNode;
    public NodeFixture EndNode { get; } = endNode;
    public MaterialFixture Material { get; } = material;
    public SectionProfileFixture SectionProfile { get; } = sectionProfile;

    /// <summary>
    /// counter-clockwise rotation in radians when looking in the negative (local) x direction
    /// </summary>
    public Angle SectionProfileRotation { get; }
}

//public class Element1dFixture(
//    string startNodeId,
//    string endNodeId,
//    string materialId,
//    string sectionProfileId,
//    Angle sectionProfileRotation,
//    Pressure modulusOfElasticity,
//    Pressure modulusOfRigidity,
//    Area area,
//    AreaMomentOfInertia strongAxisMomentOfInertia,
//    AreaMomentOfInertia weakAxisMomentOfInertia,
//    AreaMomentOfInertia polarMomentOfInertia,
//    Point startNodeLocation,
//    Point endNodeLocation
//)
//{
//    protected string StartNodeId { get; } = startNodeId;
//    protected string EndNodeId { get; } = endNodeId;
//    protected string MaterialId { get; } = materialId;
//    protected string SectionProfileId { get; } = sectionProfileId;
//    protected Angle SectionProfileRotation { get; } = sectionProfileRotation;
//    protected Pressure ModulusOfElasticity { get; } = modulusOfElasticity;
//    protected Pressure ModulusOfRigidity { get; } = modulusOfRigidity;
//    protected Area Area { get; } = area;
//    protected AreaMomentOfInertia StrongAxisMomentOfInertia { get; } = strongAxisMomentOfInertia;
//    protected AreaMomentOfInertia WeakAxisMomentOfInertia { get; } = weakAxisMomentOfInertia;
//    protected AreaMomentOfInertia PolarMomentOfInertia { get; } = polarMomentOfInertia;
//    protected Point StartNodeLocation { get; } = startNodeLocation;
//    protected Point EndNodeLocation { get; } = endNodeLocation;

//    //public CreateElement1dRequest GetCreateElement1DRequest()
//}
