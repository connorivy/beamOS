using UnitsNet;

namespace BeamOS.Tests.Common.Fixtures;

public class SectionProfileFixture(
    Area area,
    AreaMomentOfInertia strongAxisMomentOfInertia,
    AreaMomentOfInertia weakAxisMomentOfInertia,
    AreaMomentOfInertia polarMomentOfInertia
) : FixtureBase
{
    public Area Area { get; } = area;
    public AreaMomentOfInertia StrongAxisMomentOfInertia { get; } = strongAxisMomentOfInertia;
    public AreaMomentOfInertia WeakAxisMomentOfInertia { get; } = weakAxisMomentOfInertia;
    public AreaMomentOfInertia PolarMomentOfInertia { get; } = polarMomentOfInertia;
}

public class SectionProfileFixture2 : FixtureBase2
{
    public required Guid ModelId { get; init; }
    public required Area Area { get; init; }
    public required AreaMomentOfInertia StrongAxisMomentOfInertia { get; init; }
    public required AreaMomentOfInertia WeakAxisMomentOfInertia { get; init; }
    public required AreaMomentOfInertia PolarMomentOfInertia { get; init; }
}
