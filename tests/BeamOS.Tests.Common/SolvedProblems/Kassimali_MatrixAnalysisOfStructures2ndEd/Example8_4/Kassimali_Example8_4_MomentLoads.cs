using BeamOS.Tests.Common.SolvedProblems.Fixtures;
using MathNet.Spatial.Euclidean;

namespace BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

internal class Kassimali_Example8_4_MomentLoads
{
    public static MomentLoadFixture MomentLoad1 { get; } =
        new(
            Kassimali_Example8_4_Nodes.Node1,
            new(-1800, UnitsNet.Units.TorqueUnit.KilopoundForceInch),
            new(1, 0, 0)
        );

    public static MomentLoadFixture MomentLoad2 { get; } =
        new(
            Kassimali_Example8_4_Nodes.Node1,
            new(1800, UnitsNet.Units.TorqueUnit.KilopoundForceInch),
            new(0, 0, 1)
        );

    public static MomentLoadFixture MomentLoad3 { get; } =
        new(
            Kassimali_Example8_4_Nodes.Node1,
            new(3 * 20 * 20 / 12, UnitsNet.Units.TorqueUnit.KilopoundForceFoot),
            new(0, 0, 1)
        );

    public static MomentLoadFixture MomentLoad4 { get; } =
        new(
            Kassimali_Example8_4_Nodes.Node2,
            new(3 * 20 * 20 / 12, UnitsNet.Units.TorqueUnit.KilopoundForceFoot),
            new(0, 0, -1)
        );
}
