using BeamOS.Tests.Common.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

internal class Kassimali_Example8_4_Materials
{
    public static MaterialFixture2 Steel29000Ksi { get; } =
        new()
        {
            ModulusOfElasticity = new UnitsNet.Pressure(
                29000,
                UnitsNet.Units.PressureUnit.KilopoundForcePerSquareInch
            ),
            ModulusOfRigidity = new UnitsNet.Pressure(
                11500,
                UnitsNet.Units.PressureUnit.KilopoundForcePerSquareInch
            ),
            ModelId = Kassimali_Example8_4.IdStatic
        };
}
