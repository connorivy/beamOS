using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
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
                DisplacementAlongX = new(-1.3522, LengthUnit.Inch),
                DisplacementAlongY = new(-2.7965, LengthUnit.Inch),
                DisplacementAlongZ = new(-1.812, LengthUnit.Inch),
                RotationAboutX = new(-3.0021, AngleUnit.Radian),
                RotationAboutY = new(1.0569, AngleUnit.Radian),
                RotationAboutZ = new(6.4986, AngleUnit.Radian),
            },
        //new()
        //{
        //    NodeId = 2,
        //    ForceAlongX = new(-10.064, ForceUnitContract.KilopoundForce),
        //    ForceAlongY = new(-13.419, ForceUnitContract.KilopoundForce)
        //},
        //new()
        //{
        //    NodeId = 3,
        //    ForceAlongX = new(0, ForceUnitContract.KilopoundForce),
        //    ForceAlongY = new(126.83, ForceUnitContract.KilopoundForce)
        //},
        //new()
        //{
        //    NodeId = 4,
        //    ForceAlongX = new(-139.94, ForceUnitContract.KilopoundForce),
        //    ForceAlongY = new(186.58, ForceUnitContract.KilopoundForce)
        //}
        ];

    public override IEnumerable<CreateNodeRequest> Nodes()
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

    public override IEnumerable<CreateMaterialRequest> Materials()
    {
        yield return new()
        {
            ModulusOfElasticity = new(29000, PressureUnitContract.KilopoundForcePerSquareInch),
            ModulusOfRigidity = new(11500, PressureUnitContract.KilopoundForcePerSquareInch),
            Id = 1
        };
    }

    public override IEnumerable<CreateSectionProfileRequest> SectionProfiles()
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

    public override IEnumerable<CreatePointLoadRequest> PointLoads()
    {
        yield return new()
        {
            Force = new(30, ForceUnitContract.KilopoundForce),
            Direction = new(0, -1, 0),
            NodeId = 1
        };

        yield return new()
        {
            Force = new(30, ForceUnitContract.KilopoundForce),
            Direction = new(0, -1, 0),
            NodeId = 2
        };
    }

    public override IEnumerable<CreateElement1dRequest> Element1ds()
    {
        yield return new()
        {
            StartNodeId = 2,
            EndNodeId = 1,
            MaterialId = 1,
            SectionProfileId = 1
        };

        yield return new()
        {
            StartNodeId = 3,
            EndNodeId = 1,
            MaterialId = 1,
            SectionProfileId = 1
        };

        yield return new()
        {
            StartNodeId = 4,
            EndNodeId = 1,
            MaterialId = 1,
            SectionProfileId = 1
        };
    }
}
