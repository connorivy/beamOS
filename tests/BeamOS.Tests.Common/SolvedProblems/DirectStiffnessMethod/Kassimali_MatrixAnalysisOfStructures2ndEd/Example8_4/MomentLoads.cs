using BeamOS.Tests.Common.SolvedProblems.Fixtures;
using MathNet.Spatial.Euclidean;

namespace BeamOS.Tests.Common.SolvedProblems.DirectStiffnessMethod.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

internal class MomentLoads
{
    public static MomentLoadFixture MomentLoad1 { get; } =
        new(
            Nodes.Node1,
            new(-1800, UnitsNet.Units.TorqueUnit.KilopoundForceInch),
            UnitVector3D.Create(1, 0, 0)
        );

    public static MomentLoadFixture MomentLoad2 { get; } =
        new(
            Nodes.Node1,
            new(1800, UnitsNet.Units.TorqueUnit.KilopoundForceInch),
            UnitVector3D.Create(0, 0, 1)
        );

    public static MomentLoadFixture MomentLoad3 { get; } =
        new(
            Nodes.Node1,
            new(3 * 20 * 20 / 12, UnitsNet.Units.TorqueUnit.KilopoundForceFoot),
            UnitVector3D.Create(0, 0, 1)
        );

    public static MomentLoadFixture MomentLoad4 { get; } =
        new(
            Nodes.Node2,
            new(3 * 20 * 20 / 12, UnitsNet.Units.TorqueUnit.KilopoundForceFoot),
            UnitVector3D.Create(0, 0, -1)
        );
}
