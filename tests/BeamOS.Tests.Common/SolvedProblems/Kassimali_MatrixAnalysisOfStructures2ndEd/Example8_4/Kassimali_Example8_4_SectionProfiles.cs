using BeamOS.Tests.Common.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

internal class Kassimali_Example8_4_SectionProfiles
{
    public static SectionProfileFixture Profile33in2 { get; } =
        new(
            new(32.9, UnitsNet.Units.AreaUnit.SquareInch),
            new(716, UnitsNet.Units.AreaMomentOfInertiaUnit.InchToTheFourth),
            new(236, UnitsNet.Units.AreaMomentOfInertiaUnit.InchToTheFourth),
            new(15.1, UnitsNet.Units.AreaMomentOfInertiaUnit.InchToTheFourth)
        );
}
