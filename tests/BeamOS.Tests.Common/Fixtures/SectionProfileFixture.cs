using UnitsNet;

namespace BeamOS.Tests.Common.Fixtures;

public class SectionProfileFixture2 : FixtureBase2
{
    public required Guid ModelId { get; init; }
    public required Area Area { get; init; }
    public required AreaMomentOfInertia StrongAxisMomentOfInertia { get; init; }
    public required AreaMomentOfInertia WeakAxisMomentOfInertia { get; init; }
    public required AreaMomentOfInertia PolarMomentOfInertia { get; init; }
    public required Area StrongAxisShearArea { get; init; }
    public required Area WeakAxisShearArea { get; init; }
}
