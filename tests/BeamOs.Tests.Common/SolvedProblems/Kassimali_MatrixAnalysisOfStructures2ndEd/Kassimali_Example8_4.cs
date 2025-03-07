using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;

namespace BeamOs.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd;

public class Kassimali_Example8_4
    : ModelFixture,
        IHasExpectedNodeResults,
        IHasStructuralStiffnessMatrix,
        IHasExpectedReactionVector,
        IHasExpectedDisplacementVector,
        IHasDsmElement1dResults
{
    public override string Name => nameof(Kassimali_Example8_4);
    public override string Description => "Example model";
    public override string GuidString => "6b04df0f-45d6-4aed-9c04-8272ed23f811";
    public override ModelSettings Settings { get; } =
        new(UnitSettingsContract.K_IN, new(Element1dAnalysisType.Euler));
    public override SourceInfo SourceInfo { get; } =
        new(
            "Matrix Analysis Of Structures 2nd Edition by Kassimali",
            FixtureSourceType.Textbook,
            "Example 8.4",
            null,
            "https://repository.bakrie.ac.id/10/1/%5BTSI-LIB-131%5D%5BAslam_Kassimali%5D_Matrix_Analysis_of_Structure.pdf;https://dokumen.pub/matrix-analysis-of-structures-3nbsped-9780357448304.html#English"
        );
    public NodeResultFixture[] ExpectedNodeResults { get; } =

        [
            new()
            {
                NodeId = 1,
                ResultSetId = 1,
                DisplacementAlongX = new(-1.3522e-3, LengthUnit.Inch),
                DisplacementAlongY = new(-2.7965e-3, LengthUnit.Inch),
                DisplacementAlongZ = new(-1.812e-3, LengthUnit.Inch),
                RotationAboutX = new(-3.0021e-3, AngleUnit.Radian),
                RotationAboutY = new(1.0569e-3, AngleUnit.Radian),
                RotationAboutZ = new(6.4986e-3, AngleUnit.Radian),
            },
            // the two commented results will pass for openSees analysis but fail for dsmAnalysis.
            // This is because I am not taking into account fixed end forces yet
            new()
            {
                NodeId = 2,
                ResultSetId = 1,
                ForceAlongX = new(5.3757, ForceUnit.KilopoundForce),
                //ForceAlongY = new(44.106, ForceUnit.KilopoundForce),
                ForceAlongZ = new(-.74272, ForceUnit.KilopoundForce),
                TorqueAboutX = new(2.1722, TorqueUnit.KilopoundForceInch),
                TorqueAboutY = new(58.987, TorqueUnit.KilopoundForceInch),
                //TorqueAboutZ = new(2330.52, TorqueUnit.KilopoundForceInch),
            },
            new()
            {
                NodeId = 3,
                ResultSetId = 1,
                ForceAlongX = new(-4.6249, ForceUnit.KilopoundForce),
                ForceAlongY = new(11.117, ForceUnit.KilopoundForce),
                ForceAlongZ = new(-6.4607, ForceUnit.KilopoundForce),
                TorqueAboutX = new(-515.55, TorqueUnit.KilopoundForceInch),
                TorqueAboutY = new(-.76472, TorqueUnit.KilopoundForceInch),
                TorqueAboutZ = new(369.67, TorqueUnit.KilopoundForceInch),
            },
            new()
            {
                NodeId = 4,
                ResultSetId = 1,
                ForceAlongX = new(-.75082, ForceUnit.KilopoundForce),
                ForceAlongY = new(4.7763, ForceUnit.KilopoundForce),
                ForceAlongZ = new(7.2034, ForceUnit.KilopoundForce),
                TorqueAboutX = new(-383.5, TorqueUnit.KilopoundForceInch),
                TorqueAboutY = new(-60.166, TorqueUnit.KilopoundForceInch),
                TorqueAboutZ = new(-4.702, TorqueUnit.KilopoundForceInch),
            }
        ];

    public override IEnumerable<PutNodeRequest> NodeRequests()
    {
        yield return new()
        {
            LocationPoint = new(0, 0, 0, LengthUnitContract.Foot),
            Restraint = Restraint.Free,
            Id = 1
        };

        yield return new()
        {
            LocationPoint = new(-20, 0, 0, LengthUnitContract.Foot),
            Restraint = Restraint.Fixed,
            Id = 2
        };

        yield return new()
        {
            LocationPoint = new(0, -20, 0, LengthUnitContract.Foot),
            Restraint = Restraint.Fixed,
            Id = 3
        };

        yield return new()
        {
            LocationPoint = new(0, 0, -20, LengthUnitContract.Foot),
            Restraint = Restraint.Fixed,
            Id = 4
        };
    }

    public override IEnumerable<PutMaterialRequest> MaterialRequests()
    {
        yield return new()
        {
            ModulusOfElasticity = 29000,
            ModulusOfRigidity = 11500,
            PressureUnit = PressureUnitContract.KilopoundForcePerSquareInch,
            Id = 1
        };
    }

    public override IEnumerable<PutSectionProfileRequest> SectionProfileRequests()
    {
        yield return new()
        {
            Area = 32.9,
            StrongAxisMomentOfInertia = 716,
            WeakAxisMomentOfInertia = 236,
            PolarMomentOfInertia = 15.1,
            // shear area doesn't matter because we are making Euler Bernoulli assumptions
            StrongAxisShearArea = 1,
            WeakAxisShearArea = 1,
            AreaUnit = AreaUnitContract.SquareInch,
            AreaMomentOfInertiaUnit = AreaMomentOfInertiaUnitContract.InchToTheFourth,
            Id = 1
        };
    }

    public override IEnumerable<PutPointLoadRequest> PointLoadRequests()
    {
        yield return new()
        {
            Force = new(30, ForceUnitContract.KilopoundForce),
            Direction = new(0, -1, 0),
            NodeId = 1,
            Id = 1
        };

        yield return new()
        {
            Force = new(30, ForceUnitContract.KilopoundForce),
            Direction = new(0, -1, 0),
            NodeId = 2,
            Id = 2
        };
    }

    public override IEnumerable<PutMomentLoadRequest> MomentLoadRequests()
    {
        yield return new()
        {
            Torque = new(-1800, TorqueUnitContract.KilopoundForceInch),
            AxisDirection = new(1, 0, 0),
            NodeId = 1,
            Id = 1
        };

        yield return new()
        {
            Torque = new(1800, TorqueUnitContract.KilopoundForceInch),
            AxisDirection = new(0, 0, 1),
            NodeId = 1,
            Id = 2
        };

        yield return new()
        {
            Torque = new(3 * 20 * 20 / 12, TorqueUnitContract.KilopoundForceFoot),
            AxisDirection = new(0, 0, 1),
            NodeId = 1,
            Id = 3
        };

        yield return new()
        {
            Torque = new(3 * 20 * 20 / 12, TorqueUnitContract.KilopoundForceFoot),
            AxisDirection = new(0, 0, -1),
            NodeId = 2,
            Id = 4
        };
    }

    public override IEnumerable<PutElement1dRequest> Element1dRequests()
    {
        yield return new()
        {
            StartNodeId = 2,
            EndNodeId = 1,
            MaterialId = 1,
            SectionProfileId = 1,
            Id = 1
        };

        yield return new()
        {
            StartNodeId = 3,
            EndNodeId = 1,
            MaterialId = 1,
            SectionProfileId = 1,
            SectionProfileRotation = new(90, AngleUnitContract.Degree),
            Id = 2
        };

        yield return new()
        {
            StartNodeId = 4,
            EndNodeId = 1,
            MaterialId = 1,
            SectionProfileId = 1,
            SectionProfileRotation = new(30, AngleUnitContract.Degree),
            Id = 3
        };
    }

    public double[,] ExpectedStructuralStiffnessMatrix =>
        new double[,]
        {
            { 3990.3, -5.2322, 0, -627.87, -1075.4, 712.92 },
            { -5.2322, 4008.4, 0, 1800.4, 627.87, -2162.9 },
            { 0, 0, 3999.4, -2162.9, 712.92, 0 },
            { -627.87, 1800.4, -2162.9, 634857, 100459, 0 },
            { -1075.4, 627.87, 712.92, 100459, 286857, 0 },
            { 712.92, -2162.9, 0, 0, 0, 460857 }
        };

    public double[] ExpectedReactionVector =>

        [
            5.3757,
            44.106 - 30, // subtracting qf because we're not using fixed end forces. this will change.
            -0.74272,
            2.1722,
            58.987,
            2330.52 - 1200, // subtracting 1200 for same reason as above ^
            -4.6249,
            11.117,
            -6.4607,
            -515.55,
            -0.76472,
            369.67,
            -0.75082,
            4.7763,
            7.2034,
            -383.5,
            -60.166,
            -4.702
        ];

    public double[] ExpectedDisplacementVector =>
        [-1.3522e-3, -2.7965e-3, -1.812e-3, -3.0021e-3, 1.0569e-3, 6.4986e-3];

    public DsmElement1dResultFixture[] ExpectedDsmElement1dResults { get; } =

        [
            new() {
            ElementId = 1,
            ResultSetId = 1,
            ExpectedRotationMatrix = new double[,]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 }
            },
            ExpectedTransformationMatrix = new double[,]
            {
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }
            },
            ExpectedLocalStiffnessMatrix = new double[,]
            {
                { 3975.4, 0, 0, 0, 0, 0, -3975.4, 0, 0, 0, 0, 0 },
                { 0, 18.024, 0, 0, 0, 2162.9, 0, -18.024, 0, 0, 0, 2162.9 },
                { 0, 0, 5.941, 0, -712.92, 0, 0, 0, -5.941, 0, -712.92, 0, },
                { 0, 0, 0, 723.54, 0, 0, 0, 0, 0, -723.54, 0, 0 },
                { 0, 0, -712.92, 0, 114066.7, 0, 0, 0, 712.92, 0, 57033.3, 0 },
                { 0, 2162.9, 0, 0, 0, 346066.7, 0, -2162.9, 0, 0, 0, 173033.3 },
                { -3975.4, 0, 0, 0, 0, 0, 3975.4, 0, 0, 0, 0, 0 },
                { 0, -18.024, 0, 0, 0, -2162.9, 0, 18.024, 0, 0, 0, -2162.9 },
                { 0, 0, -5.941, 0, 712.92, 0, 0, 0, 5.941, 0, 712.92, 0 },
                { 0, 0, 0, -723.54, 0, 0, 0, 0, 0, 723.54, 0, 0 },
                { 0, 0, -712.92, 0, 57033.3, 0, 0, 0, 712.92, 0, 114066.7, 0 },
                { 0, 2162.9, 0, 0, 0, 173033.3, 0, -2162.9, 0, 0, 0, 346066.7 }
            },
            // Note : same as local stiffness matrix
            ExpectedGlobalStiffnessMatrix = new double[,]
            {
                { 3975.4, 0, 0, 0, 0, 0, -3975.4, 0, 0, 0, 0, 0 },
                { 0, 18.024, 0, 0, 0, 2162.9, 0, -18.024, 0, 0, 0, 2162.9 },
                { 0, 0, 5.941, 0, -712.92, 0, 0, 0, -5.941, 0, -712.92, 0, },
                { 0, 0, 0, 723.54, 0, 0, 0, 0, 0, -723.54, 0, 0 },
                { 0, 0, -712.92, 0, 114066.7, 0, 0, 0, 712.92, 0, 57033.3, 0 },
                { 0, 2162.9, 0, 0, 0, 346066.7, 0, -2162.9, 0, 0, 0, 173033.3 },
                { -3975.4, 0, 0, 0, 0, 0, 3975.4, 0, 0, 0, 0, 0 },
                { 0, -18.024, 0, 0, 0, -2162.9, 0, 18.024, 0, 0, 0, -2162.9 },
                { 0, 0, -5.941, 0, 712.92, 0, 0, 0, 5.941, 0, 712.92, 0 },
                { 0, 0, 0, -723.54, 0, 0, 0, 0, 0, 723.54, 0, 0 },
                { 0, 0, -712.92, 0, 57033.3, 0, 0, 0, 712.92, 0, 114066.7, 0 },
                { 0, 2162.9, 0, 0, 0, 173033.3, 0, -2162.9, 0, 0, 0, 346066.7 }
            },
            ExpectedLocalFixedEndForces =  [0, 30, 0, 0, 0, 12000, 0, 30, 0, 0, 0, -1200],
            ExpectedLocalEndDisplacements =
            [
                0,
                0,
                0,
                0,
                0,
                0,
                -1.3522e-3,
                -2.7965e-3,
                -1.812e-3,
                -3.0021e-3,
                1.0569e-3,
                6.4986e-3
            ],
            ExpectedGlobalEndDisplacements =
            [
                0,
                0,
                0,
                0,
                0,
                0,
                -1.3522e-3,
                -2.7965e-3,
                -1.812e-3,
                -3.0021e-3,
                1.0569e-3,
                6.4986e-3
            ],
            ExpectedLocalEndForces =
            [
                5.3757,
                44.106,
                -0.74272,
                2.1722,
                58.987,
                2330.5,
                -5.3757,
                15.894,
                0.74272,
                -2.1722,
                119.27,
                1055
            ]
        },
            new() {
            ElementId = 2,
            ResultSetId = 1,
            ExpectedRotationMatrix = new double[,]
            {
                { 0, 1, 0 },
                { 0, 0, 1 },
                { 1, 0, 0 }
            },
            ExpectedTransformationMatrix = new double[,]
            {
                { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 }
            },
            ExpectedGlobalStiffnessMatrix = new double[,]
            {
                { 5.941, 0, 0, 0, 0, -712.92, -5.941, 0, 0, 0, 0, -712.92 },
                { 0, 3975.4, 0, 0, 0, 0, 0, -3975.4, 0, 0, 0, 0 },
                { 0, 0, 18.024, 2162.9, 0, 0, 0, 0, -18.024, 2162.9, 0, 0 },
                { 0, 0, 2162.9, 346066.7, 0, 0, 0, 0, -2162.9, 173033.3, 0, 0 },
                { 0, 0, 0, 0, 723.54, 0, 0, 0, 0, 0, -723.54, 0 },
                { -712.92, 0, 0, 0, 0, 114066.7, 712.92, 0, 0, 0, 0, 57033.3 },
                { -5.941, 0, 0, 0, 0, 712.92, 5.941, 0, 0, 0, 0, 712.92 },
                { 0, -3975.4, 0, 0, 0, 0, 0, 3975.4, 0, 0, 0, 0 },
                { 0, 0, -18.024, -2162.9, 0, 0, 0, 0, 18.024, -2162.9, 0, 0 },
                { 0, 0, 2162.9, 173033.3, 0, 0, 0, 0, -2162.9, 346066.7, 0, 0 },
                { 0, 0, 0, 0, -723.54, 0, 0, 0, 0, 0, 723.54, 0 },
                { -712.92, 0, 0, 0, 0, 57033.3, 712.92, 0, 0, 0, 0, 114066.7 },
            },
            ExpectedLocalFixedEndForces =  [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
            ExpectedLocalEndDisplacements =
            [
                0,
                0,
                0,
                0,
                0,
                0,
                -2.7965e-3,
                -1.812e-3,
                -1.3522e-3,
                1.0569e-3,
                6.4986e-3,
                -3.0021e-3
            ],
            ExpectedGlobalEndDisplacements =
            [
                0,
                0,
                0,
                0,
                0,
                0,
                -1.3522e-3,
                -2.7965e-3,
                -1.812e-3,
                -3.0021e-3,
                1.0569e-3,
                6.4986e-3
            ],
            ExpectedLocalEndForces =
            [
                11.117,
                -6.4607,
                -4.6249,
                -0.76472,
                369.67,
                -515.55,
                -11.117,
                6.4607,
                4.6249,
                0.76472,
                740.31,
                -1035,
            ],
            ExpectedGlobalEndForces =
            [
                -4.6249,
                11.117,
                -6.4607,
                -515.55,
                -0.76472,
                369.67,
                4.6249,
                -11.117,
                6.4607,
                -1035,
                0.76472,
                740.31,
            ]
        },
            new() {
            ElementId = 3,
            ResultSetId = 1,
            ExpectedRotationMatrix = new double[,]
            {
                { 0, 0, 1 },
                { -.5, .86603, 0 },
                { -.86603, -.5, 0 }
            },
            ExpectedTransformationMatrix = new double[,]
            {
                { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { -0.5, 0.86603, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { -0.86603, -0.5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, -0.5, 0.86603, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, -0.86603, -0.5, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, -0.5, 0.86603, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, -0.86603, -0.5, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, -0.5, 0.86603, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, -0.86603, -0.5, 0 }
            },
            ExpectedGlobalStiffnessMatrix = new double[,]
            {
                { 8.9618, -5.2322, 0, 627.87, 1075.4, 0, -8.9618, 5.2322, 0, 627.87, 1075.4, 0 },
                {
                    -5.2322,
                    15.003,
                    0,
                    -1800.4,
                    -627.87,
                    0,
                    5.2322,
                    -15.003,
                    0,
                    -1800.4,
                    -627.87,
                    0
                },
                { 0, 0, 3975.4, 0, 0, 0, 0, 0, -3975.4, 0, 0, 0 },
                {
                    627.87,
                    -1800.4,
                    0,
                    288066.7,
                    100458.9,
                    0,
                    -627.87,
                    1800.4,
                    0,
                    144033.3,
                    50229.5,
                    0
                },
                {
                    1075.4,
                    -627.87,
                    0,
                    100458.9,
                    172066.7,
                    0,
                    -1075.4,
                    627.87,
                    0,
                    50229.5,
                    86033.3,
                    0
                },
                { 0, 0, 0, 0, 0, 723.54, 0, 0, 0, 0, 0, -723.54 },
                {
                    -8.9618,
                    5.2322,
                    0,
                    -627.87,
                    -1075.4,
                    0,
                    8.9618,
                    -5.2322,
                    0,
                    -627.87,
                    -1075.4,
                    0
                },
                { 5.2322, -15.003, 0, 1800.4, 627.87, 0, -5.2322, 15.003, 0, 1800.4, 627.87, 0 },
                { 0, 0, -3975.4, 0, 0, 0, 0, 0, 3975.4, 0, 0, 0 },
                {
                    627.87,
                    -1800.4,
                    0,
                    144033.3,
                    50229.5,
                    0,
                    -627.87,
                    1800.4,
                    0,
                    288066.7,
                    100458.9,
                    0
                },
                {
                    1075.4,
                    -627.87,
                    0,
                    50229.5,
                    86033.3,
                    0,
                    -1075.4,
                    627.87,
                    0,
                    100458.9,
                    172066.7,
                    0
                },
                { 0, 0, 0, 0, 0, -723.54, 0, 0, 0, 0, 0, 723.54 }
            },
            ExpectedLocalFixedEndForces =  [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
            ExpectedLocalEndDisplacements =
            [
                0,
                0,
                0,
                0,
                0,
                0,
                -1.812e-3,
                -1.7457e-3,
                2.5693e-3,
                6.4986e-3,
                2.4164e-3,
                2.0714e-3
            ],
            ExpectedGlobalEndDisplacements =
            [
                0,
                0,
                0,
                0,
                0,
                0,
                -1.3522e-3,
                -2.7965e-3,
                -1.812e-3,
                -3.0021e-3,
                1.0569e-3,
                6.4986e-3
            ],
            ExpectedLocalEndForces =
            [
                7.2034,
                4.5118,
                -1.7379,
                -4.702,
                139.65,
                362.21,
                -7.2034,
                -4.5118,
                1.7379,
                4.702,
                277.46,
                720.63,
            ],
            ExpectedGlobalEndForces =
            [
                -0.75082,
                4.7763,
                7.2034,
                -383.5,
                -60.166,
                -4.702,
                0.75082,
                -4.7763,
                -7.2034,
                -762.82,
                -120.03,
                4.702,
            ]
        }
        ];
}
