using BeamOS.Common.Domain.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate;
using BeamOS.DirectStiffnessMethod.Domain.Common.ValueObjects;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.SolvedProblems.MatrixAnalysisOfStructures2ndEd;

internal partial class Example8_4
{
    public static AnalyticalNode AnalyticalNode1 { get; }
    public static AnalyticalNode AnalyticalNode2 { get; }
    public static AnalyticalNode AnalyticalNode3 { get; }
    public static AnalyticalNode AnalyticalNode4 { get; }

    private static AnalyticalNode GetAnalyticalNode1()
    {
        List<PointLoad> pointLoads =
        [
            new(
                new Force(-30, ForceUnit.KilopoundForce),
                DenseVector.OfArray([0, 1, 0 ])
            )
        ];

        List<MomentLoad> momentLoads =
        [
            new MomentLoad(
                new Torque(-1800, TorqueUnit.KilopoundForceInch),
                DenseVector.OfArray([1, 0, 0])
            ),
            new MomentLoad(
                new Torque(1800, TorqueUnit.KilopoundForceInch),
                DenseVector.OfArray([0, 0, 1])
            ),
            // this moment load represents the fixed end moment of the distributed load.
            // this load should be removed when there is support for fixed-end moments.
            new MomentLoad(
                new Torque(3 * 20 * 20 / 12, TorqueUnit.KilopoundForceFoot),
                DenseVector.OfArray([0, 0, 1])
            ),
        ];
        return new(
            0,
            0,
            0,
            LengthUnit.Foot,
            Restraint.Free,
            pointLoads: pointLoads,
            momentLoads: momentLoads,
            id: new(Constants.Guid1)
        );
    }

    private static AnalyticalNode GetAnalyticalNode2()
    {
        List<PointLoad> pointLoads =
        [
            new(
                new Force(-30, ForceUnit.KilopoundForce),
                DenseVector.OfArray([0, 1, 0 ])
            )
        ];

        List<MomentLoad> momentLoads =
        [
            // this moment load represents the fixed end moment of the distributed load.
            // this load should be removed when there is support for fixed-end moments.
            new MomentLoad(
                new Torque(3 * 20 * 20 / 12, TorqueUnit.KilopoundForceFoot),
                DenseVector.OfArray([0, 0, -1])
            ),
        ];

        return new(-20, 0, 0, LengthUnit.Foot, Restraint.Fixed, id: new(Constants.Guid2));
    }

    private static AnalyticalNode GetAnalyticalNode3()
    {
        return new(0, -20, 0, LengthUnit.Foot, Restraint.Fixed, id: new(Constants.Guid3));
    }

    private static AnalyticalNode GetAnalyticalNode4()
    {
        return new(0, 0, -20, LengthUnit.Foot, Restraint.Fixed, id: new(Constants.Guid4));
    }
}