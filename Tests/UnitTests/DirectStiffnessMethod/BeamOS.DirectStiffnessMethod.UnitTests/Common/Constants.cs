using BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate.ValueObjects;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common;
public static partial class Constants
{
    public static AnalyticalModelSettings DefaultSettingsK_IN { get; } = new(UnitSettings.K_IN);
    public static AnalyticalModelSettings DefaultSettingsSI { get; } = new(UnitSettings.SI);
    public static Material UnitMaterialSI { get; } = new(
      modulusOfElasticity: new(1, UnitSystem.SI),
      modulusOfRigidity: new(1, UnitSystem.SI)
    );
    public static SectionProfile UnitSectionProfileSI { get; } = new(
      new Area(1, UnitSystem.SI),
      new AreaMomentOfInertia(1, UnitSystem.SI),
      new AreaMomentOfInertia(1, UnitSystem.SI),
      new AreaMomentOfInertia(1, UnitSystem.SI)
    );
}
