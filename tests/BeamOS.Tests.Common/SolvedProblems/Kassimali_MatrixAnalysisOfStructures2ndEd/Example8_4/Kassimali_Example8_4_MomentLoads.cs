using BeamOS.Tests.Common.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

internal class Kassimali_Example8_4_MomentLoads
{
    public static MomentLoadFixture2 MomentLoad1 { get; } =
        new()
        {
            Torque = new(-1800, UnitsNet.Units.TorqueUnit.KilopoundForceInch),
            AxisDirection = new(1, 0, 0),
            ModelId = Kassimali_Example8_4.IdStatic,
            Node = new(() => Kassimali_Example8_4_Nodes.Node1)
        };

    public static MomentLoadFixture2 MomentLoad2 { get; } =
        new()
        {
            Torque = new(1800, UnitsNet.Units.TorqueUnit.KilopoundForceInch),
            AxisDirection = new(0, 0, 1),
            ModelId = Kassimali_Example8_4.IdStatic,
            Node = new(() => Kassimali_Example8_4_Nodes.Node1)
        };

    public static MomentLoadFixture2 MomentLoad3 { get; } =
        new()
        {
            Torque = new(3 * 20 * 20 / 12, UnitsNet.Units.TorqueUnit.KilopoundForceFoot),
            AxisDirection = new(0, 0, 1),
            ModelId = Kassimali_Example8_4.IdStatic,
            Node = new(() => Kassimali_Example8_4_Nodes.Node1)
        };

    public static MomentLoadFixture2 MomentLoad4 { get; } =
        new()
        {
            Torque = new(3 * 20 * 20 / 12, UnitsNet.Units.TorqueUnit.KilopoundForceFoot),
            AxisDirection = new(0, 0, -1),
            ModelId = Kassimali_Example8_4.IdStatic,
            Node = new(() => Kassimali_Example8_4_Nodes.Node2)
        };

    public static MomentLoadFixture2[] All { get; } =
        [MomentLoad1, MomentLoad2, MomentLoad3, MomentLoad4];
}
