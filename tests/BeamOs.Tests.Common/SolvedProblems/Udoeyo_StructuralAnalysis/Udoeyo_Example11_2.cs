using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;

namespace BeamOs.Tests.Common.SolvedProblems.Udoeyo_StructuralAnalysis;

public class Udoeyo_Example11_2 : ModelFixture, IHasExpectedNodeResults
{
    public override SourceInfo SourceInfo => SourceInfos.Udoeyo with { ModelName = "Example 11.2" };

    public override string Name => nameof(Udoeyo_Example11_2);

    public override string Description => "Example 11.2 from Udoeyo's Structural Analysis book";

    public override ModelSettings Settings { get; } =
        new(UnitSettings.K_FT)
        {
            AnalysisSettings = new() { Element1DAnalysisType = Element1dAnalysisType.Euler },
        };

    public override string GuidString => "97fd5585-d80c-4680-836c-4d4c5c9c3c61";

    public NodeResultFixture[] ExpectedNodeResults { get; } =
        [
            new NodeResultFixture()
            {
                NodeId = 1,
                ResultSetId = 1,
                ForceAlongY = new(15.46, ForceUnit.KilopoundForce),
                TorqueAboutZ = new(-23.4, TorqueUnit.KilopoundForceFoot),
            },
            new NodeResultFixture()
            {
                NodeId = 4,
                ResultSetId = 1,
                ForceAlongY = new(8.54 + 14.5, ForceUnit.KilopoundForce),
                // TorqueAboutZ = new(0, TorqueUnit.KilopoundForceFoot),
                TorqueAboutZ = new(-29.9, TorqueUnit.KilopoundForceFoot),
            },
            new NodeResultFixture()
            {
                NodeId = 5,
                ResultSetId = 1,
                TorqueAboutZ = new(57.1, TorqueUnit.KilopoundForceFoot),
            },
            new NodeResultFixture()
            {
                NodeId = 2,
                ResultSetId = 1,
                ForceAlongY = new(9.5, ForceUnit.KilopoundForce),
                TorqueAboutZ = new(0, TorqueUnit.KilopoundForceFoot),
            },
        ];

    public override IEnumerable<PutElement1dRequest> Element1dRequests()
    {
        yield return new PutElement1dRequest()
        {
            Id = 1,
            StartNodeId = 1,
            EndNodeId = 2,
            MaterialId = 1,
            SectionProfileId = 1,
        };
    }

    public override IEnumerable<LoadCase> LoadCaseRequests()
    {
        yield return new LoadCase() { Id = 1, Name = "Dead Load" };
    }

    public override IEnumerable<LoadCombination> LoadCombinationRequests()
    {
        yield return new LoadCombination()
        {
            Id = 1,
            LoadCaseFactors = new Dictionary<int, double> { { 1, 1.0 } },
        };
    }

    public override IEnumerable<PutMaterialRequest> MaterialRequests()
    {
        yield return new PutMaterialRequest()
        {
            Id = 1,
            ModulusOfElasticity = 1,
            ModulusOfRigidity = 1,
            PressureUnit = PressureUnitContract.KilopoundForcePerSquareFoot,
        };
    }

    public override IEnumerable<SectionProfileFromLibrary> SectionProfilesFromLibraryRequests()
    {
        yield return new SectionProfileFromLibrary()
        {
            Id = 1,
            Name = "W12x40",
            Library = StructuralCode.AISC_360_16,
        };
    }

    public override IEnumerable<PutNodeRequest> NodeRequests()
    {
        yield return new PutNodeRequest()
        {
            Id = 1,
            LocationPoint = new Point(0, 0, 0, LengthUnitContract.Foot),
            Restraint = Restraint.Fixed,
        };
        yield return new PutNodeRequest()
        {
            Id = 2,
            LocationPoint = new Point(24, 0, 0, LengthUnitContract.Foot),
            Restraint = Restraint.PinnedXyPlane,
        };
    }

    public override IEnumerable<InternalNode> InternalNodeRequests()
    {
        yield return new InternalNode(
            3,
            1,
            new(12.0 / 3.0 / 24.0, RatioUnit.DecimalFraction),
            Restraint.FreeXyPlane
        );
        yield return new InternalNode(
            4,
            1,
            new(0.5, RatioUnit.DecimalFraction),
            new()
            {
                CanTranslateAlongX = true,
                CanTranslateAlongY = false,
                CanTranslateAlongZ = false,
                CanRotateAboutX = false,
                CanRotateAboutY = false,
                CanRotateAboutZ = true,
            }
        );
        yield return new InternalNode(5, 1, new(0.75, RatioUnit.DecimalFraction));
    }

    public override IEnumerable<PutPointLoadRequest> PointLoadRequests()
    {
        yield return new PutPointLoadRequest()
        {
            Id = 1,
            NodeId = 3,
            LoadCaseId = 1,
            Force = new(4 * 12 / 2.0, ForceUnitContract.KilopoundForce),
            Direction = new(0, -1, 0),
        };
        yield return new PutPointLoadRequest()
        {
            Id = 2,
            NodeId = 5,
            LoadCaseId = 1,
            Force = new(24, ForceUnitContract.KilopoundForce),
            Direction = new(0, -1, 0),
        };
    }

    public override IEnumerable<PutMomentLoadRequest> MomentLoadRequests()
    {
        yield return new PutMomentLoadRequest()
        {
            Id = 1,
            NodeId = 1,
            LoadCaseId = 1,
            Torque = new(4 * 12 * 12 / 20.0, TorqueUnitContract.KilopoundForceFoot),
            AxisDirection = new(0, 0, 1),
        };
        yield return new PutMomentLoadRequest()
        {
            Id = 2,
            NodeId = 4,
            LoadCaseId = 1,
            Torque = new(-4 * 12 * 12 / 30.0, TorqueUnitContract.KilopoundForceFoot),
            AxisDirection = new(0, 0, 1),
        };
    }
}
