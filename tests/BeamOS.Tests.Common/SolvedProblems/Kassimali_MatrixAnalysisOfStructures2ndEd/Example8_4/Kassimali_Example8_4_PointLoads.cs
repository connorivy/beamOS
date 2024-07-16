using BeamOS.Tests.Common.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

internal class Kassimali_Example8_4_PointLoads
{
    public static PointLoadFixture2 PointLoad1 { get; } =
        new()
        {
            Force = new(30, UnitsNet.Units.ForceUnit.KilopoundForce),
            Direction = new(0, -1, 0),
            ModelId = Kassimali_Example8_4.IdStatic,
            Node = new(() => Kassimali_Example8_4_Nodes.Node1)
        };

    public static PointLoadFixture2 PointLoad2 { get; } =
        new()
        {
            Force = new(30, UnitsNet.Units.ForceUnit.KilopoundForce),
            Direction = new(0, -1, 0),
            ModelId = Kassimali_Example8_4.IdStatic,
            Node = new(() => Kassimali_Example8_4_Nodes.Node1)
        };

    //private static PointLoadFixture2 pointLoad1;

    //public static PointLoadFixture2 PointLoad1(Guid nodeId) =>
    //    pointLoad1 ??= new()
    //    {
    //        NodeId = nodeId,
    //        Force = new(30, UnitsNet.Units.ForceUnit.KilopoundForce),
    //        Direction = new(0, -1, 0),
    //        ModelId = Kassimali_Example8_4.IdStatic
    //    };

    //private static PointLoadFixture2 pointLoad2;

    //public static PointLoadFixture2 PointLoad2(Guid nodeId) =>
    //    pointLoad2 ??= new()
    //    {
    //        NodeId = nodeId,
    //        Force = new(30, UnitsNet.Units.ForceUnit.KilopoundForce),
    //        Direction = new(0, -1, 0),
    //        ModelId = Kassimali_Example8_4.IdStatic
    //    };
}
