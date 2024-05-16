using BeamOs.Domain.Common.ValueObjects;
using UnitsNet;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

public class Element1dFixture(
    NodeFixture startNode,
    NodeFixture endNode,
    MaterialFixture material,
    SectionProfileFixture sectionProfile,
    UnitSettings unitSettings
) : FixtureBase
{
    public NodeFixture StartNode { get; } = startNode;
    public NodeFixture EndNode { get; } = endNode;
    public MaterialFixture Material { get; } = material;
    public SectionProfileFixture SectionProfile { get; } = sectionProfile;
    public UnitSettings UnitSettings { get; } = unitSettings;

    /// <summary>
    /// counter-clockwise rotation in radians when looking in the negative (local) x direction
    /// </summary>
    public Angle SectionProfileRotation { get; init; }
}
