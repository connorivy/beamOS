using BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalModelAggregate.ValueObjects;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common;

public static partial class Constants
{
    public static AnalyticalModelSettings DefaultSettingsK_IN { get; } = new(UnitSettings.K_IN);
    public static AnalyticalModelSettings DefaultSettingsSI { get; } = new(UnitSettings.SI);
    public static Material UnitMaterialSI { get; } =
        new(modulusOfElasticity: new(1, UnitSystem.SI), modulusOfRigidity: new(1, UnitSystem.SI));
    public static SectionProfile UnitSectionProfileSI { get; } =
        new(
            new Area(1, UnitSystem.SI),
            new AreaMomentOfInertia(1, UnitSystem.SI),
            new AreaMomentOfInertia(1, UnitSystem.SI),
            new AreaMomentOfInertia(1, UnitSystem.SI)
        );

    public static Guid Guid0 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000000");
    public static Guid Guid1 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public static Guid Guid2 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000002");
    public static Guid Guid3 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000003");
    public static Guid Guid4 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000004");
    public static Guid Guid5 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000005");
    public static Guid Guid6 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000006");
    public static Guid Guid7 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000007");
    public static Guid Guid8 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000008");
    public static Guid Guid9 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000009");
    public static Guid Guid10 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000010");
    public static Guid Guid11 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000011");
    public static Guid Guid12 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000012");
    public static Guid Guid13 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000013");
    public static Guid Guid14 { get; } = Guid.Parse("00000000-0000-0000-0000-000000000014");
}
