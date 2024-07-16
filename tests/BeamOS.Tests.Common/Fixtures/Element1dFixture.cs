using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.Domain.Common.ValueObjects;
using BeamOS.Tests.Common.Interfaces;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;
using UnitsNet;

namespace BeamOS.Tests.Common.Fixtures;

public class Element1dFixture(
    ModelFixture model,
    NodeFixture startNode,
    NodeFixture endNode,
    MaterialFixture material,
    SectionProfileFixture sectionProfile,
    UnitSettings unitSettings,
    string? elementName = null
) : FixtureBase, IModelMember, ITestFixtureDisplayable
{
    public ModelFixture Model { get; } = model;

    //public override GuidWrapperForModelId ModelId { get; } = new(modelId);
    public NodeFixture StartNode { get; } = startNode;
    public NodeFixture EndNode { get; } = endNode;
    public MaterialFixture Material { get; } = material;
    public SectionProfileFixture SectionProfile { get; } = sectionProfile;
    public UnitSettings UnitSettings { get; } = unitSettings;

    /// <summary>
    /// counter-clockwise rotation in radians when looking in the negative (local) x direction
    /// </summary>
    public Angle SectionProfileRotation { get; init; }
    public string? ElementName { get; } = elementName;

    public SourceInfo SourceInfo => this.Model.SourceInfo with { ElementName = this.ElementName };

    public async Task Display(IEditorApiAlpha editorApiAlpha)
    {
        await this.Model.Display(editorApiAlpha);
        // todo : isolate element
    }
}

public record Element1dFixture2 : FixtureBase2
{
    public required Guid ModelId { get; init; }

    //public ModelFixture Model { get; init; }
    public required NodeFixture2 StartNode { get; init; }

    public required NodeFixture2 EndNode { get; init; }
    public required MaterialFixture2 Material { get; init; }
    public required SectionProfileFixture2 SectionProfile { get; init; }

    /// <summary>
    /// counter-clockwise rotation in radians when looking in the negative (local) x direction
    /// </summary>
    public Angle SectionProfileRotation { get; init; } = Angle.Zero;
    public string? ElementName { get; init; }
}
