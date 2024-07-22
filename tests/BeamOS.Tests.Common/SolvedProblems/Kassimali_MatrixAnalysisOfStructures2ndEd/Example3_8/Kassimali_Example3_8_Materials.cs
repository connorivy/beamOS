using BeamOS.Tests.Common.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example3_8;

internal class Kassimali_Example3_8_Materials
{
    public static MaterialFixture2 Default { get; } =
        new()
        {
            ModulusOfElasticity = new UnitsNet.Pressure(
                29000,
                UnitsNet.Units.PressureUnit.KilopoundForcePerSquareInch
            ),
            ModulusOfRigidity = new UnitsNet.Pressure(
                1,
                UnitsNet.Units.PressureUnit.KilopoundForcePerSquareInch
            ),
            ModelId = Kassimali_Example3_8.IdStatic
        };

    public static MaterialFixture2[] All { get; } = [Default];
}
