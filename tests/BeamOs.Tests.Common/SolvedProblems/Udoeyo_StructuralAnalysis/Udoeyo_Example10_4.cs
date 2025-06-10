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

public class Udoeyo_Example10_4 : ModelFixture, IHasExpectedNodeResults
{
    public override SourceInfo SourceInfo => SourceInfos.Udoeyo with { ModelName = "Example 10.4" };

    public override string Name => nameof(Udoeyo_Example10_4);

    public override string Description => "Example 10.4 from Udoeyo's Structural Analysis book";

    public override ModelSettings Settings { get; } =
        new(UnitSettings.kN_M)
        {
            AnalysisSettings = new() { Element1DAnalysisType = Element1dAnalysisType.Euler },
            YAxisUp = true,
        };

    public override string GuidString => "ab081a4f-89ac-4a45-a4db-bd253703df7d";

    public NodeResultFixture[] ExpectedNodeResults { get; } =
        [
            new NodeResultFixture()
            {
                NodeId = 1,
                ResultSetId = 1,
                ForceAlongY = new(37.5, ForceUnit.Kilonewton),
                TorqueAboutZ = new(0, TorqueUnit.KilonewtonMeter),
            },
            new NodeResultFixture()
            {
                NodeId = 3,
                ResultSetId = 1,
                ForceAlongY = new(125, ForceUnit.Kilonewton),
                TorqueAboutZ = new(0, TorqueUnit.KilonewtonMeter),
            },
            new NodeResultFixture()
            {
                NodeId = 2,
                ResultSetId = 1,
                ForceAlongY = new(37.5, ForceUnit.Kilonewton),
                TorqueAboutZ = new(0, TorqueUnit.KilonewtonMeter),
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
        yield return new LoadCombination()
        {
            Id = 2,
            LoadCaseFactors = new Dictionary<int, double> { { 1, 1.0 } },
        };
    }

    public override IEnumerable<PutSectionProfileRequest> SectionProfileRequests()
    {
        yield return new PutSectionProfileRequest()
        {
            Id = 1,
            Name = "Constant",
            Area = 1,
            PolarMomentOfInertia = 1,
            StrongAxisMomentOfInertia = 1,
            StrongAxisPlasticSectionModulus = 1,
            WeakAxisMomentOfInertia = 1,
            WeakAxisPlasticSectionModulus = 1,
            LengthUnit = LengthUnitContract.Meter,
        };
    }

    public override IEnumerable<PutMaterialRequest> MaterialRequests()
    {
        yield return new PutMaterialRequest()
        {
            Id = 1,
            ModulusOfElasticity = 1,
            ModulusOfRigidity = 1,
            PressureUnit = PressureUnitContract.KilonewtonPerSquareMeter,
        };
    }

    public override IEnumerable<PutNodeRequest> NodeRequests()
    {
        yield return new PutNodeRequest()
        {
            Id = 1,
            LocationPoint = new Point(0, 0, 0, LengthUnitContract.Meter),
            Restraint = Restraint.PinnedXyPlane,
        };
        yield return new PutNodeRequest()
        {
            Id = 2,
            LocationPoint = new Point(10, 0, 0, LengthUnitContract.Meter),
            Restraint = new()
            {
                CanTranslateAlongX = true,
                CanTranslateAlongY = false,
                CanTranslateAlongZ = false,
                CanRotateAboutX = false,
                CanRotateAboutY = false,
                CanRotateAboutZ = true,
            },
        };
    }

    public override IEnumerable<InternalNode> InternalNodeRequests()
    {
        yield return new InternalNode(
            3,
            1,
            new(.5, RatioUnit.DecimalFraction),
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
    }

    public override IEnumerable<PutPointLoadRequest> PointLoadRequests()
    {
        yield return new PutPointLoadRequest()
        {
            Id = 1,
            NodeId = 1,
            LoadCaseId = 1,
            Force = new(20 * 5 / 2.0, ForceUnitContract.Kilonewton),
            Direction = new(0, -1, 0),
        };
        yield return new PutPointLoadRequest()
        {
            Id = 2,
            NodeId = 2,
            LoadCaseId = 1,
            Force = new(20 * 5 / 2.0, ForceUnitContract.Kilonewton),
            Direction = new(0, -1, 0),
        };
        yield return new PutPointLoadRequest()
        {
            Id = 3,
            NodeId = 3,
            LoadCaseId = 1,
            Force = new(20 * 5, ForceUnitContract.Kilonewton),
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
            Torque = new(-20 * 5 * 5 / 12.0, TorqueUnitContract.KilonewtonMeter),
            AxisDirection = new(0, 0, 1),
        };
        yield return new PutMomentLoadRequest()
        {
            Id = 2,
            NodeId = 2,
            LoadCaseId = 1,
            Torque = new(20 * 5 * 5 / 12.0, TorqueUnitContract.KilonewtonMeter),
            AxisDirection = new(0, 0, 1),
        };
    }
}
