using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.DirectStiffnessMethod.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

internal class SectionProfiles
{
    public static SectionProfileFixture Profile33in2 { get; } =
        new(
            new(32.9, UnitsNet.Units.AreaUnit.SquareInch),
            new(716, UnitsNet.Units.AreaMomentOfInertiaUnit.InchToTheFourth),
            new(236, UnitsNet.Units.AreaMomentOfInertiaUnit.InchToTheFourth),
            new(15.1, UnitsNet.Units.AreaMomentOfInertiaUnit.InchToTheFourth)
        );
}
