using BeamOS.Tests.Common.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

internal class Kassimali_Example8_4_SectionProfiles
{
    public static SectionProfileFixture2 Profile33in2 { get; } =
        new()
        {
            Area = new(32.9, UnitsNet.Units.AreaUnit.SquareInch),
            StrongAxisMomentOfInertia = new(
                716,
                UnitsNet.Units.AreaMomentOfInertiaUnit.InchToTheFourth
            ),
            WeakAxisMomentOfInertia = new(
                236,
                UnitsNet.Units.AreaMomentOfInertiaUnit.InchToTheFourth
            ),
            PolarMomentOfInertia = new(
                15.1,
                UnitsNet.Units.AreaMomentOfInertiaUnit.InchToTheFourth
            ),
            ModelId = Kassimali_Example8_4.IdStatic
        };

    public static SectionProfileFixture2[] All { get; } = [Profile33in2];
}
