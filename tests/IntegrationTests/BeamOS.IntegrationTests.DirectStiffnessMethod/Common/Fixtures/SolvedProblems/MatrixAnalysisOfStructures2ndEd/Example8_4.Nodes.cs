using BeamOS.Common.Domain.ValueObjects;
using BeamOS.PhysicalModel.Domain.MomentLoadAggregate;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate;
using BeamOS.Tests.Common;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures.SolvedProblems.MatrixAnalysisOfStructures2ndEd;

internal partial class Example8_4
{
    //public static Node Node1 { get; }
    //public static Node Node2 { get; }
    //public static Node Node3 { get; }
    //public static Node Node4 { get; }

    //private static Node GetNode1()
    //{
    //    List<PointLoad> pointLoads =
    //    [
    //        new(
    //            modelId,
    //            new Force(-30, ForceUnit.KilopoundForce),
    //            DenseVector.OfArray([0, 1, 0 ])
    //        )
    //    ];

    //    List<MomentLoad> momentLoads =
    //    [
    //        new MomentLoad(
    //            new Torque(-1800, TorqueUnit.KilopoundForceInch),
    //            DenseVector.OfArray([1, 0, 0])
    //        ),
    //        new MomentLoad(
    //            new Torque(1800, TorqueUnit.KilopoundForceInch),
    //            DenseVector.OfArray([0, 0, 1])
    //        ),
    //        // this moment load represents the fixed end moment of the distributed load.
    //        // this load should be removed when there is support for fixed-end moments.
    //        new MomentLoad(
    //            new Torque(3 * 20 * 20 / 12, TorqueUnit.KilopoundForceFoot),
    //            DenseVector.OfArray([0, 0, 1])
    //        ),
    //    ];
    //    return new(
    //        0,
    //        0,
    //        0,
    //        LengthUnit.Foot,
    //        Restraint.Free,
    //        pointLoads: pointLoads,
    //        momentLoads: momentLoads,
    //        id: new(Constants.Guid1)
    //    );
    //}

    //private static Node GetNode2()
    //{
    //    List<PointLoad> pointLoads =
    //    [
    //        new(
    //            new Force(-30, ForceUnit.KilopoundForce),
    //            DenseVector.OfArray([0, 1, 0 ])
    //        )
    //    ];

    //    List<MomentLoad> momentLoads =
    //    [
    //        // this moment load represents the fixed end moment of the distributed load.
    //        // this load should be removed when there is support for fixed-end moments.
    //        new MomentLoad(
    //            new Torque(3 * 20 * 20 / 12, TorqueUnit.KilopoundForceFoot),
    //            DenseVector.OfArray([0, 0, -1])
    //        ),
    //    ];

    //    return new(-20, 0, 0, LengthUnit.Foot, Restraint.Fixed, id: new(Constants.Guid2));
    //}

    //private static Node GetNode3()
    //{
    //    return new(0, -20, 0, LengthUnit.Foot, Restraint.Fixed, id: new(Constants.Guid3));
    //}

    //private static Node GetNode4()
    //{
    //    return new(0, 0, -20, LengthUnit.Foot, Restraint.Fixed, id: new(Constants.Guid4));
    //}
}
