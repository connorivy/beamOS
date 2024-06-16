using UnitsNet;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

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
