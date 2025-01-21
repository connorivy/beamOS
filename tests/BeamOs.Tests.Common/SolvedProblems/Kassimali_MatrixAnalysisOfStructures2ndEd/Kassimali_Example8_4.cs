using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;
using UnitsNet.Units;

namespace BeamOs.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd;

public class Kassimali_Example8_4 : ModelFixture, IHasExpectedNodeResults
{
    public override string Name => nameof(Kassimali_Example8_4);
    public override string Description => "Example model";
    public override string GuidString => "6b04df0f-45d6-4aed-9c04-8272ed23f811";
    public override PhysicalModelSettings Settings { get; } =
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
                DisplacementAlongX = new(-1.3522e-3, LengthUnit.Inch),
                DisplacementAlongY = new(-2.7965e-3, LengthUnit.Inch),
                DisplacementAlongZ = new(-1.812e-3, LengthUnit.Inch),
                RotationAboutX = new(-3.0021e-3, AngleUnit.Radian),
                RotationAboutY = new(1.0569e-3, AngleUnit.Radian),
                RotationAboutZ = new(6.4986e-3, AngleUnit.Radian),
            },
            new()
            {
                NodeId = 2,
                ForceAlongX = new(5.3757, ForceUnit.KilopoundForce),
                ForceAlongY = new(44.106, ForceUnit.KilopoundForce),
                ForceAlongZ = new(-.74272, ForceUnit.KilopoundForce),
                TorqueAboutX = new(2.1722, TorqueUnit.KilopoundForceInch),
                TorqueAboutY = new(58.987, TorqueUnit.KilopoundForceInch),
                TorqueAboutZ = new(2330.52, TorqueUnit.KilopoundForceInch),
            },
            new()
            {
                NodeId = 3,
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
                ForceAlongX = new(-.75082, ForceUnit.KilopoundForce),
                ForceAlongY = new(4.7763, ForceUnit.KilopoundForce),
                ForceAlongZ = new(7.2034, ForceUnit.KilopoundForce),
                TorqueAboutX = new(-383.5, TorqueUnit.KilopoundForceInch),
                TorqueAboutY = new(-60.166, TorqueUnit.KilopoundForceInch),
                TorqueAboutZ = new(-4.702, TorqueUnit.KilopoundForceInch),
            }
        ];

    public override IEnumerable<CreateNodeRequest> NodeRequests()
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

    public override IEnumerable<CreateMaterialRequest> MaterialRequests()
    {
        yield return new()
        {
            ModulusOfElasticity = new(29000, PressureUnitContract.KilopoundForcePerSquareInch),
            ModulusOfRigidity = new(11500, PressureUnitContract.KilopoundForcePerSquareInch),
            Id = 1
        };
    }

    public override IEnumerable<CreateSectionProfileRequest> SectionProfileRequests()
    {
        yield return new()
        {
            Area = new(32.9, AreaUnitContract.SquareInch),
            StrongAxisMomentOfInertia = new(716, AreaMomentOfInertiaUnitContract.InchToTheFourth),
            WeakAxisMomentOfInertia = new(236, AreaMomentOfInertiaUnitContract.InchToTheFourth),
            PolarMomentOfInertia = new(15.1, AreaMomentOfInertiaUnitContract.InchToTheFourth),
            // shear area doesn't matter because we are making Euler Bernoulli assumptions
            StrongAxisShearArea = new(1, AreaUnitContract.SquareInch),
            WeakAxisShearArea = new(1, AreaUnitContract.SquareInch),
            Id = 1
        };
    }

    public override IEnumerable<CreatePointLoadRequest> PointLoadRequests()
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

    public override IEnumerable<CreateMomentLoadRequest> MomentLoadRequests()
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

    public override IEnumerable<CreateElement1dRequest> Element1dRequests()
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
}
