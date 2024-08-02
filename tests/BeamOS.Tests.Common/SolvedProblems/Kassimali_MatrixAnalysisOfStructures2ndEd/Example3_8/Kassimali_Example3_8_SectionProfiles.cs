using BeamOS.Tests.Common.Fixtures;
using UnitsNet.Units;

namespace BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example3_8;

internal class Kassimali_Example3_8_SectionProfiles
{
    public static SectionProfileFixture2 Profile8in2 { get; } =
        new()
        {
            Area = new(8, UnitsNet.Units.AreaUnit.SquareInch),
            StrongAxisMomentOfInertia = new(
                1,
                UnitsNet.Units.AreaMomentOfInertiaUnit.InchToTheFourth
            ),
            WeakAxisMomentOfInertia = new(
                1,
                UnitsNet.Units.AreaMomentOfInertiaUnit.InchToTheFourth
            ),
            PolarMomentOfInertia = new(1, UnitsNet.Units.AreaMomentOfInertiaUnit.InchToTheFourth),
            StrongAxisShearArea = new(1, AreaUnit.SquareInch),
            WeakAxisShearArea = new(1, AreaUnit.SquareInch),
            ModelId = Kassimali_Example3_8.IdStatic
        };

    public static SectionProfileFixture2 Profile6in2 { get; } =
        new()
        {
            Area = new(6, UnitsNet.Units.AreaUnit.SquareInch),
            StrongAxisMomentOfInertia = new(
                1,
                UnitsNet.Units.AreaMomentOfInertiaUnit.InchToTheFourth
            ),
            WeakAxisMomentOfInertia = new(
                1,
                UnitsNet.Units.AreaMomentOfInertiaUnit.InchToTheFourth
            ),
            PolarMomentOfInertia = new(1, UnitsNet.Units.AreaMomentOfInertiaUnit.InchToTheFourth),
            StrongAxisShearArea = new(1, AreaUnit.SquareInch),
            WeakAxisShearArea = new(1, AreaUnit.SquareInch),
            ModelId = Kassimali_Example3_8.IdStatic
        };

    public static SectionProfileFixture2[] All { get; } = [Profile8in2, Profile6in2];
}
