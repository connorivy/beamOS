using BeamOS.DirectStiffnessMethod.Domain.Element1DAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.ModelAggregate.Enums;
using BeamOS.DirectStiffnessMethod.Domain.ModelAggregate.ValueObjects;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common;
public static partial class Constants
{
    public static ModelSettings DefaultSettingsK_IN { get; } = new(
      UnitSettings.K_IN,
      ModelOrientation.ZUp,
      new Length(.005, UnitSystem.SI),
      new Length(5, UnitSystem.SI),
      10
      );
    public static ModelSettings DefaultSettingsSI { get; } = new(
      UnitSettings.SI,
      ModelOrientation.ZUp,
      new Length(.005, UnitSystem.SI),
      new Length(5, UnitSystem.SI),
      10
      );
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