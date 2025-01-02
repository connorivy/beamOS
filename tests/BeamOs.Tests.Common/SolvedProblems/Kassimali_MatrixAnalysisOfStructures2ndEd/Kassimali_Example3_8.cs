using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;
using UnitsNet.Units;

namespace BeamOs.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd;

public class Kassimali_Example3_8 : ModelFixture, IHasExpectedNodeResults
{
    public override string Name => nameof(Kassimali_Example3_8);
    public override string Description => "Example model";
    public override string GuidString => "0a83df88-656e-47d9-98fe-25fd7d370b06";
    public override PhysicalModelSettings Settings { get; } =
        new(UnitSettingsContract.K_IN, new(Element1dAnalysisType.Euler));
    public override SourceInfo SourceInfo { get; } =
        new(
            "Matrix Analysis Of Structures 2nd Edition by Kassimali",
            FixtureSourceType.Textbook,
            "Example 3.8",
            null,
            "https://repository.bakrie.ac.id/10/1/%5BTSI-LIB-131%5D%5BAslam_Kassimali%5D_Matrix_Analysis_of_Structure.pdf;https://dokumen.pub/matrix-analysis-of-structures-3nbsped-9780357448304.html#English"
        );
    public NodeResultFixture[] ExpectedNodeResults { get; } =

        [
            new()
            {
                NodeId = 1,
                DisplacementAlongX = new(.21552, LengthUnit.Inch),
                DisplacementAlongY = new(-.13995, LengthUnit.Inch)
            },
            new()
            {
                NodeId = 2,
                ForceAlongX = new(-10.064, ForceUnit.KilopoundForce),
                ForceAlongY = new(-13.419, ForceUnit.KilopoundForce)
            },
            new()
            {
                NodeId = 3,
                ForceAlongX = new(0, ForceUnit.KilopoundForce),
                ForceAlongY = new(126.83, ForceUnit.KilopoundForce)
            },
            new()
            {
                NodeId = 4,
                ForceAlongX = new(-139.94, ForceUnit.KilopoundForce),
                ForceAlongY = new(186.58, ForceUnit.KilopoundForce)
            }
        ];

    private static readonly Restraint free2D = new(true, true, false, false, false, true);
    private static readonly Restraint pinned2d = new(false, false, false, false, false, true);

    public override IEnumerable<CreateNodeRequest> Nodes()
    {
        yield return new()
        {
            LocationPoint = new(12, 16, 0, LengthUnitContract.Foot),
            Restraint = free2D,
            Id = 1
        };

        yield return new()
        {
            LocationPoint = new(0, 0, 0, LengthUnitContract.Foot),
            Restraint = pinned2d,
            Id = 2
        };

        yield return new()
        {
            LocationPoint = new(12, 0, 0, LengthUnitContract.Foot),
            Restraint = pinned2d,
            Id = 3
        };

        yield return new()
        {
            LocationPoint = new(24, 0, 0, LengthUnitContract.Foot),
            Restraint = pinned2d,
            Id = 4
        };
    }

    public override IEnumerable<CreateMaterialRequest> Materials()
    {
        yield return new()
        {
            ModulusOfElasticity = new(29000, PressureUnitContract.KilopoundForcePerSquareInch),
            ModulusOfRigidity = new(1, PressureUnitContract.KilopoundForcePerSquareInch),
            Id = 992
        };
    }

    public override IEnumerable<CreateSectionProfileRequest> SectionProfiles()
    {
        yield return new()
        {
            Area = new(8, AreaUnitContract.SquareInch),
            StrongAxisMomentOfInertia = new(1, AreaMomentOfInertiaUnitContract.InchToTheFourth),
            WeakAxisMomentOfInertia = new(1, AreaMomentOfInertiaUnitContract.InchToTheFourth),
            PolarMomentOfInertia = new(1, AreaMomentOfInertiaUnitContract.InchToTheFourth),
            StrongAxisShearArea = new(1, AreaUnitContract.SquareInch),
            WeakAxisShearArea = new(1, AreaUnitContract.SquareInch),
            Id = 8
        };

        yield return new()
        {
            Area = new(6, AreaUnitContract.SquareInch),
            StrongAxisMomentOfInertia = new(1, AreaMomentOfInertiaUnitContract.InchToTheFourth),
            WeakAxisMomentOfInertia = new(1, AreaMomentOfInertiaUnitContract.InchToTheFourth),
            PolarMomentOfInertia = new(1, AreaMomentOfInertiaUnitContract.InchToTheFourth),
            StrongAxisShearArea = new(1, AreaUnitContract.SquareInch),
            WeakAxisShearArea = new(1, AreaUnitContract.SquareInch),
            Id = 6
        };
    }

    public override IEnumerable<CreatePointLoadRequest> PointLoads()
    {
        yield return new()
        {
            Force = new(150, ForceUnitContract.KilopoundForce),
            Direction = new(1, 0, 0),
            NodeId = 1
        };

        yield return new()
        {
            Force = new(300, ForceUnitContract.KilopoundForce),
            Direction = new(0, -1, 0),
            NodeId = 1
        };
    }

    public override IEnumerable<CreateElement1dRequest> Element1ds()
    {
        yield return new()
        {
            StartNodeId = 2,
            EndNodeId = 1,
            MaterialId = 992,
            SectionProfileId = 8
        };

        yield return new()
        {
            StartNodeId = 3,
            EndNodeId = 1,
            MaterialId = 992,
            SectionProfileId = 6
        };

        yield return new()
        {
            StartNodeId = 4,
            EndNodeId = 1,
            MaterialId = 992,
            SectionProfileId = 8
        };
    }
}
