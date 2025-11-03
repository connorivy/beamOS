using System;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;

namespace BeamOs.Tests.Common.SolvedProblems.SAP2000;

public class Simple2dFrame : ModelFixture
//, IHasExpectedNodeResults
{
    public override SourceInfo SourceInfo { get; } =
        new("SAP2000", FixtureSourceType.SAP2000, nameof(Simple2dFrame));

    public override string Name => nameof(Simple2dFrame);

    public override string Description => "A simple 2D frame model created in SAP2000.";

    public override ModelSettings Settings { get; } =
        new(
            new()
            {
                LengthUnit = LengthUnitContract.Foot,
                ForceUnit = ForceUnitContract.KilopoundForce,
            },
            null,
            new() { ModelingMode = ModelingMode.Independent },
            true
        );

    public override string GuidString => "e5606cea-015c-4443-bb80-d038670bd005";

    public override IEnumerable<PutElement1dRequest> Element1dRequests()
    {
        yield return new PutElement1dRequest
        {
            Id = 1,
            StartNodeId = 1,
            EndNodeId = 2,
            SectionProfileId = 1,
            MaterialId = 1,
        };
        yield return new PutElement1dRequest
        {
            Id = 2,
            StartNodeId = 2,
            EndNodeId = 4,
            SectionProfileId = 1,
            MaterialId = 1,
        };
        yield return new PutElement1dRequest
        {
            Id = 3,
            StartNodeId = 4,
            EndNodeId = 5,
            SectionProfileId = 1,
            MaterialId = 1,
        };
    }

    public override IEnumerable<LoadCase> LoadCaseRequests()
    {
        yield return new LoadCase { Id = 1, Name = "Dead Load" };
    }

    public override IEnumerable<LoadCombination> LoadCombinationRequests()
    {
        yield return new LoadCombination
        {
            Id = 1,
            LoadCaseFactors = new Dictionary<int, double>
            {
                { 1, 1.0 }, // Dead Load
            },
        };
    }

    public override IEnumerable<PutMaterialRequest> MaterialRequests()
    {
        yield return new PutMaterialRequest
        {
            Id = 1,
            ModulusOfElasticity = 29000,
            ModulusOfRigidity = 11600,
            PressureUnit = PressureUnitContract.KilopoundForcePerSquareInch,
        };
    }

    public override IEnumerable<PutNodeRequest> NodeRequests()
    {
        yield return new PutNodeRequest
        {
            Id = 1,
            LocationPoint = new(0, 0, 0, LengthUnitContract.Foot),
            Restraint = Restraint.Fixed,
        };
        yield return new PutNodeRequest
        {
            Id = 2,
            LocationPoint = new(0, 10, 0, LengthUnitContract.Foot),
            Restraint = Restraint.FreeXyPlane,
        };
        yield return new PutNodeRequest
        {
            Id = 4,
            LocationPoint = new(10, 10, 0, LengthUnitContract.Foot),
            Restraint = Restraint.FreeXyPlane,
        };
        yield return new PutNodeRequest
        {
            Id = 5,
            LocationPoint = new(10, 0, 0, LengthUnitContract.Foot),
            Restraint = Restraint.Fixed,
        };
    }

    public override IEnumerable<InternalNode> InternalNodeRequests()
    {
        yield return new InternalNode(3, 1, new(50, RatioUnitContract.Percent));
    }

    public override IEnumerable<SectionProfileFromLibrary> SectionProfilesFromLibraryRequests()
    {
        yield return new SectionProfileFromLibrary
        {
            Id = 1,
            Name = "W12x35",
            Library = StructuralCode.AISC_360_16,
        };
    }

    public override IEnumerable<PutPointLoadRequest> PointLoadRequests()
    {
        yield return new PutPointLoadRequest
        {
            Id = 1,
            NodeId = 3,
            LoadCaseId = 1,
            Force = new(5, ForceUnitContract.KilopoundForce),
            Direction = new(0, -1, 0),
        };
    }
}
