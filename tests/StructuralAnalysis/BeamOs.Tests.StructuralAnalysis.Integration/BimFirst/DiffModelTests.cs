using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Sdk;
using FluentAssertions;

namespace BeamOs.Tests.StructuralAnalysis.Integration.Api;

public class DiffModelTests
{
    private static BeamOsDynamicModel DynamicModel()
    {
        return new BeamOsDynamicModel(
            Guid.NewGuid(),
            new() { UnitSettings = UnitSettingsContract.K_FT },
            "test model",
            "test model"
        );
    }

    [Test]
    public async Task InitialDiff_ShouldReturnEmptyDifferences()
    {
        BeamOsDynamicModel modelA = DynamicModel();
        BeamOsDynamicModel modelB = DynamicModel();

        modelA.AddNode(1, 0, 0, 0);
        modelB.AddNode(1, 0, 0, 0);

        modelA.AddNode(2, 0, 0, 10);
        modelB.AddNode(2, 0, 0, 10);

        modelA.AddNode(3, 0, 0, 20);
        modelB.AddNode(3, 0, 0, 20);

        modelA.AddMaterial(1, 290000, 11500);
        modelB.AddMaterial(1, 290000, 11500);

        modelA.AddSectionProfileFromLibrary(1, "W8X10", StructuralCode.AISC_360_16);
        modelB.AddSectionProfileFromLibrary(1, "W8X10", StructuralCode.AISC_360_16);

        modelA.AddElement1d(1, 1, 2, 1, 1);
        modelB.AddElement1d(1, 1, 2, 1, 1);

        modelA.AddElement1d(2, 2, 3, 1, 1);
        modelB.AddElement1d(2, 2, 3, 1, 1);

        await modelA.CreateOnly(AssemblySetup.StructuralAnalysisRemoteApiClient);
        await modelB.CreateOnly(AssemblySetup.StructuralAnalysisRemoteApiClient);

        var diffRequest = new DiffModelRequest() { TargetModelId = modelB.Id };

        var diffResponse = await AssemblySetup
            .StructuralAnalysisRemoteApiClient.Models[modelA.Id]
            .Diff.GetModelDiffAsync(diffRequest);

        diffResponse.ThrowIfError();

        diffResponse.Value.Element1ds.Should().BeEmpty();
        diffResponse.Value.Nodes.Should().BeEmpty();
        diffResponse.Value.Materials.Should().BeEmpty();
        diffResponse.Value.SectionProfiles.Should().BeEmpty();
    }

    [Test]
    public async Task DiffWithAddedNode_ShouldDetectAddedNode()
    {
        BeamOsDynamicModel modelA = DynamicModel();
        BeamOsDynamicModel modelB = DynamicModel();

        modelA.AddNode(1, 0, 0, 0);
        modelB.AddNode(1, 0, 0, 0);

        modelA.AddNode(2, 0, 0, 10);
        modelB.AddNode(2, 0, 0, 10);

        modelA.AddNode(3, 0, 0, 20);
        modelB.AddNode(3, 0, 0, 20);

        modelA.AddMaterial(1, 290000, 11500);
        modelB.AddMaterial(1, 290000, 11500);

        modelA.AddSectionProfileFromLibrary(1, "W8X10", StructuralCode.AISC_360_16);
        modelB.AddSectionProfileFromLibrary(1, "W8X10", StructuralCode.AISC_360_16);

        modelA.AddElement1d(1, 1, 2, 1, 1);
        modelB.AddElement1d(1, 1, 2, 1, 1);

        modelA.AddElement1d(2, 2, 3, 1, 1);
        modelB.AddElement1d(2, 2, 3, 1, 1);

        modelB.AddNode(4, 0, 0, 30);

        await modelA.CreateOnly(AssemblySetup.StructuralAnalysisRemoteApiClient);
        await modelB.CreateOnly(AssemblySetup.StructuralAnalysisRemoteApiClient);

        var diffRequest = new DiffModelRequest() { TargetModelId = modelB.Id };

        var diffResponse = await AssemblySetup
            .StructuralAnalysisRemoteApiClient.Models[modelA.Id]
            .Diff.GetModelDiffAsync(diffRequest);

        diffResponse.ThrowIfError();

        diffResponse.Value.Nodes.Should().HaveCount(1);
        diffResponse.Value.Nodes[0].Status.Should().Be(DiffStatus.Added);
        diffResponse.Value.Nodes[0].TargetEntity.Id.Should().Be(4);
        diffResponse.Value.Element1ds.Should().BeEmpty();
    }

    [Test]
    public async Task DiffWithRemovedElement_ShouldDetectRemovedElement()
    {
        BeamOsDynamicModel modelA = DynamicModel();
        BeamOsDynamicModel modelB = DynamicModel();

        modelA.AddNode(1, 0, 0, 0);
        modelB.AddNode(1, 0, 0, 0);

        modelA.AddNode(2, 0, 0, 10);
        modelB.AddNode(2, 0, 0, 10);

        modelA.AddNode(3, 0, 0, 20);
        modelB.AddNode(3, 0, 0, 20);

        modelA.AddMaterial(1, 290000, 11500);
        modelB.AddMaterial(1, 290000, 11500);

        modelA.AddSectionProfileFromLibrary(1, "W8X10", StructuralCode.AISC_360_16);
        modelB.AddSectionProfileFromLibrary(1, "W8X10", StructuralCode.AISC_360_16);

        modelA.AddElement1d(1, 1, 2, 1, 1);
        modelB.AddElement1d(1, 1, 2, 1, 1);

        modelA.AddElement1d(2, 2, 3, 1, 1);
        // modelB.AddElement1d(2, 2, 3, 1, 1);

        await modelA.CreateOnly(AssemblySetup.StructuralAnalysisRemoteApiClient);
        await modelB.CreateOnly(AssemblySetup.StructuralAnalysisRemoteApiClient);

        var diffRequest = new DiffModelRequest() { TargetModelId = modelB.Id };

        var diffResponse = await AssemblySetup
            .StructuralAnalysisRemoteApiClient.Models[modelA.Id]
            .Diff.GetModelDiffAsync(diffRequest);

        diffResponse.ThrowIfError();

        diffResponse.Value.Element1ds.Should().HaveCount(1);
        diffResponse.Value.Element1ds[0].Status.Should().Be(DiffStatus.Removed);
        diffResponse.Value.Element1ds[0].SourceEntity.Id.Should().Be(2);
        diffResponse.Value.Nodes.Should().BeEmpty();
    }

    [Test]
    public async Task DiffWithModifiedElement_ShouldDetectModifiedElement()
    {
        BeamOsDynamicModel modelA = DynamicModel();
        BeamOsDynamicModel modelB = DynamicModel();

        modelA.AddNode(1, 0, 0, 0);
        modelB.AddNode(1, 0, 0, 0);

        modelA.AddNode(2, 0, 0, 10);
        modelB.AddNode(2, 0, 0, 10);

        modelA.AddNode(3, 0, 0, 20);
        modelB.AddNode(3, 0, 0, 20);

        modelB.AddNode(4, 0, 0, 30);

        modelA.AddMaterial(1, 290000, 11500);
        modelB.AddMaterial(1, 290000, 11500);

        modelA.AddSectionProfileFromLibrary(1, "W8X10", StructuralCode.AISC_360_16);
        modelB.AddSectionProfileFromLibrary(1, "W8X10", StructuralCode.AISC_360_16);

        modelA.AddElement1d(1, 1, 2, 1, 1);
        modelB.AddElement1d(1, 1, 2, 1, 1);

        modelA.AddElement1d(2, 2, 3, 1, 1);
        modelB.AddElement1d(2, 2, 4, 1, 1);

        await modelA.CreateOnly(AssemblySetup.StructuralAnalysisRemoteApiClient);
        await modelB.CreateOnly(AssemblySetup.StructuralAnalysisRemoteApiClient);

        var diffRequest = new DiffModelRequest() { TargetModelId = modelB.Id };

        var diffResponse = await AssemblySetup
            .StructuralAnalysisRemoteApiClient.Models[modelA.Id]
            .Diff.GetModelDiffAsync(diffRequest);

        diffResponse.ThrowIfError();

        diffResponse.Value.Nodes.Should().HaveCount(1);
        diffResponse.Value.Nodes[0].Status.Should().Be(DiffStatus.Added);

        diffResponse.Value.Element1ds.Should().HaveCount(1);
        diffResponse.Value.Element1ds[0].Status.Should().Be(DiffStatus.Modified);
        diffResponse.Value.Element1ds[0].TargetEntity.Id.Should().Be(2);
        diffResponse.Value.Element1ds[0].TargetEntity.StartNodeId.Should().Be(2);
        diffResponse.Value.Element1ds[0].SourceEntity.EndNodeId.Should().Be(3);
        diffResponse.Value.Element1ds[0].TargetEntity.EndNodeId.Should().Be(4);
    }

    [Test]
    public async Task DiffWithAddedMaterial_ShouldDetectAddedMaterial()
    {
        BeamOsDynamicModel modelA = DynamicModel();
        BeamOsDynamicModel modelB = DynamicModel();

        modelA.AddNode(1, 0, 0, 0);
        modelB.AddNode(1, 0, 0, 0);

        modelA.AddNode(2, 0, 0, 10);
        modelB.AddNode(2, 0, 0, 10);

        modelA.AddNode(3, 0, 0, 20);
        modelB.AddNode(3, 0, 0, 20);

        modelA.AddMaterial(1, 290000, 11500);
        modelB.AddMaterial(1, 290000, 11500);

        modelA.AddSectionProfileFromLibrary(1, "W8X10", StructuralCode.AISC_360_16);
        modelB.AddSectionProfileFromLibrary(1, "W8X10", StructuralCode.AISC_360_16);

        modelA.AddElement1d(1, 1, 2, 1, 1);
        modelB.AddElement1d(1, 1, 2, 1, 1);

        modelA.AddElement1d(2, 2, 3, 1, 1);
        modelB.AddElement1d(2, 2, 3, 1, 1);

        modelB.AddMaterial(2, 200000, 80000);

        await modelA.CreateOnly(AssemblySetup.StructuralAnalysisRemoteApiClient);
        await modelB.CreateOnly(AssemblySetup.StructuralAnalysisRemoteApiClient);

        var diffRequest = new DiffModelRequest() { TargetModelId = modelB.Id };

        var diffResponse = await AssemblySetup
            .StructuralAnalysisRemoteApiClient.Models[modelA.Id]
            .Diff.GetModelDiffAsync(diffRequest);

        diffResponse.ThrowIfError();

        diffResponse.Value.Materials.Should().HaveCount(1);
        diffResponse.Value.Materials[0].Status.Should().Be(DiffStatus.Added);
        diffResponse.Value.Materials[0].TargetEntity.Id.Should().Be(2);
    }

    [Test]
    public async Task CreateModelProposalFromDiff_WithAddedNodeOperations_ShouldCreateCorrectProposal()
    {
        // Arrange
        BeamOsDynamicModel modelA = DynamicModel();
        modelA.AddNode(1, 0, 0, 0);
        modelA.AddNode(2, 0, 0, 10);
        modelA.AddNode(3, 0, 0, 20);
        modelA.AddMaterial(1, 290000, 11500);
        modelA.AddSectionProfileFromLibrary(1, "W8X10", StructuralCode.AISC_360_16);
        modelA.AddElement1d(1, 1, 2, 1, 1);
        modelA.AddElement1d(2, 2, 3, 1, 1);
        await modelA.CreateOnly(AssemblySetup.StructuralAnalysisRemoteApiClient);

        var diffResponse = new ModelDiffData
        {
            BaseModelId = modelA.Id,
            TargetModelId = Guid.NewGuid(),
            Nodes =
            [
                new()
                {
                    SourceEntity = null,
                    TargetEntity = new NodeResponse
                    {
                        Id = 4,
                        ModelId = modelA.Id,
                        LocationPoint = new Point
                        {
                            X = 0,
                            Y = 0,
                            Z = 30,
                            LengthUnit = LengthUnitContract.Foot,
                        },
                        Restraint = Restraint.Free,
                    },
                },
                new()
                {
                    SourceEntity = new NodeResponse
                    {
                        Id = 2,
                        ModelId = modelA.Id,
                        LocationPoint = new Point
                        {
                            X = 0,
                            Y = 0,
                            Z = 10,
                            LengthUnit = LengthUnitContract.Foot,
                        },
                        Restraint = Restraint.Fixed,
                    },
                    TargetEntity = new NodeResponse
                    {
                        Id = 2,
                        ModelId = modelA.Id,
                        LocationPoint = new Point
                        {
                            X = 10,
                            Y = 0,
                            Z = 40,
                            LengthUnit = LengthUnitContract.Foot,
                        },
                        Restraint = Restraint.Fixed,
                    },
                },
                new()
                {
                    SourceEntity = new NodeResponse
                    {
                        Id = 3,
                        ModelId = modelA.Id,
                        LocationPoint = new Point
                        {
                            X = 0,
                            Y = 0,
                            Z = 10,
                            LengthUnit = LengthUnitContract.Foot,
                        },
                        Restraint = Restraint.Free,
                    },
                    TargetEntity = null,
                },
            ],
        };

        var modelProposalCreationResult = await AssemblySetup
            .StructuralAnalysisRemoteApiClient.Models[modelA.Id]
            .Proposals.FromDiff.CreateModelProposalFromDiffAsync(diffResponse);
        modelProposalCreationResult.ThrowIfError();

        var modelProposalResult = await AssemblySetup
            .StructuralAnalysisRemoteApiClient.Models[modelA.Id]
            .Proposals[modelProposalCreationResult.Value.Id]
            .GetModelProposalAsync();
        modelProposalResult.ThrowIfError();

        modelProposalResult.Value.CreateNodeProposals.Should().HaveCount(1);
        modelProposalCreationResult.Value.CreateNodeProposals.Should().HaveCount(1);

        var unitsNetLocation = modelProposalResult
            .Value.CreateNodeProposals[0]
            .LocationPoint.ToDomain();
        unitsNetLocation.X.Feet.Should().Be(0);
        unitsNetLocation.Y.Feet.Should().Be(0);
        unitsNetLocation.Z.Feet.Should().Be(30);
        modelProposalResult.Value.CreateNodeProposals[0].Restraint.Should().Be(Restraint.Free);

        modelProposalResult.Value.ModifyNodeProposals.Should().HaveCount(1);
        modelProposalCreationResult.Value.ModifyNodeProposals.Should().HaveCount(1);

        var modifiedUnitsNetLocation = modelProposalResult
            .Value.ModifyNodeProposals[0]
            .LocationPoint.ToDomain();
        modifiedUnitsNetLocation.X.Feet.Should().Be(10);
        modifiedUnitsNetLocation.Y.Feet.Should().Be(0);
        modifiedUnitsNetLocation.Z.Feet.Should().Be(40);
        modelProposalResult.Value.ModifyNodeProposals[0].Restraint.Should().Be(Restraint.Fixed);

        modelProposalResult.Value.DeleteModelEntityProposals.Should().HaveCount(1);
        modelProposalCreationResult.Value.DeleteModelEntityProposals.Should().HaveCount(1);

        modelProposalResult
            .Value.DeleteModelEntityProposals[0]
            .ObjectType.Should()
            .Be(BeamOsObjectType.Node);
        modelProposalResult.Value.DeleteModelEntityProposals[0].Id.Should().Be(3);
    }
}
