using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.DirectStiffnessMethod.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

internal class Materials
{
    public static MaterialFixture Steel29000Ksi { get; } =
        new MaterialFixture(
            new UnitsNet.Pressure(29000, UnitsNet.Units.PressureUnit.KilopoundForcePerSquareInch),
            new UnitsNet.Pressure(11500, UnitsNet.Units.PressureUnit.KilopoundForcePerSquareInch)
        );
}
