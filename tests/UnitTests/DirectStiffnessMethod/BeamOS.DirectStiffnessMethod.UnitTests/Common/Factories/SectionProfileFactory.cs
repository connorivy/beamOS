using BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate.ValueObjects;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Factories;
internal static class SectionProfileFactory
{
    public static SectionProfile CreateSI(
      Area? area = null,
      AreaMomentOfInertia? strongAxisMomentOfInertia = null,
      AreaMomentOfInertia? weakAxisMomentOfInertia = null,
      AreaMomentOfInertia? polarMomentOfInertia = null
    )
    {
        return new(
          area ?? new Area(1, UnitSystem.SI),
          strongAxisMomentOfInertia ?? new AreaMomentOfInertia(1, UnitSystem.SI),
          weakAxisMomentOfInertia ?? new AreaMomentOfInertia(1, UnitSystem.SI),
          polarMomentOfInertia ?? new AreaMomentOfInertia(1, UnitSystem.SI)
        );
    }
}
