using BeamOs.Domain.Common.ValueObjects;
using BeamOS.Tests.Common.Fixtures;
using UnitsNet.Units;

namespace BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example3_8;

public static class Kassimali_Example3_8_Nodes
{
    private static readonly Restraint free2D = new(true, true, false, false, false, true);
    private static readonly Restraint pinned2d = new(false, false, false, false, false, true);
    public static NodeFixture2 Node1 { get; } =
        new()
        {
            LocationPoint = new Point(12, 16, 0, LengthUnit.Foot),
            Restraint = free2D,
            ModelId = Kassimali_Example3_8.IdStatic,
            PointLoads =
            [
                Kassimali_Example3_8_PointLoads.PointLoad1,
                Kassimali_Example3_8_PointLoads.PointLoad2
            ]
        };

    public static NodeFixture2 Node2 { get; } =
        new()
        {
            LocationPoint = new Point(0, 0, 0, LengthUnit.Foot),
            Restraint = pinned2d,
            ModelId = Kassimali_Example3_8.IdStatic,
        };

    public static NodeFixture2 Node3 { get; } =
        new()
        {
            LocationPoint = new Point(12, 0, 0, LengthUnit.Foot),
            Restraint = pinned2d,
            ModelId = Kassimali_Example3_8.IdStatic
        };

    public static NodeFixture2 Node4 { get; } =
        new()
        {
            LocationPoint = new Point(24, 0, 0, LengthUnit.Foot),
            Restraint = pinned2d,
            ModelId = Kassimali_Example3_8.IdStatic
        };

    public static NodeFixture2[] All { get; } = [Node1, Node2, Node3, Node4];
}
