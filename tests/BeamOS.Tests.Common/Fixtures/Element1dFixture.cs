using BeamOS.Tests.Common.Interfaces;
using UnitsNet;

namespace BeamOS.Tests.Common.Fixtures;

public class Element1dFixture2 : FixtureBase2, IHasSourceInfo
{
    public Guid ModelId => this.Model.Value.Id;
    public required Lazy<ModelFixture2> Model { get; init; }
    public required NodeFixture2 StartNode { get; init; }

    public required NodeFixture2 EndNode { get; init; }
    public required MaterialFixture2 Material { get; init; }
    public required SectionProfileFixture2 SectionProfile { get; init; }

    /// <summary>
    /// counter-clockwise rotation in radians when looking in the negative (local) x direction
    /// </summary>
    public Angle SectionProfileRotation { get; init; } = Angle.Zero;
    public string? ElementName { get; init; }
    public SourceInfo SourceInfo =>
        this.Model.Value.SourceInfo with
        {
            ElementName = this.ElementName
        };
}
