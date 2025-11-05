using System.Collections.Concurrent;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Sdk;
using BeamOs.Tests.Common;
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
            .Diff.GitModelDiffAsync(diffRequest);

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
            .Diff.GitModelDiffAsync(diffRequest);

        diffResponse.ThrowIfError();

        diffResponse.Value.Nodes.Should().HaveCount(1);
        diffResponse.Value.Nodes[0].Status.Should().Be(DiffStatus.Added);
        diffResponse.Value.Nodes[0].Entity.Id.Should().Be(4);
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
            .Diff.GitModelDiffAsync(diffRequest);

        diffResponse.ThrowIfError();

        diffResponse.Value.Element1ds.Should().HaveCount(1);
        diffResponse.Value.Element1ds[0].Status.Should().Be(DiffStatus.Removed);
        diffResponse.Value.Element1ds[0].Entity.Id.Should().Be(2);
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
            .Diff.GitModelDiffAsync(diffRequest);

        diffResponse.ThrowIfError();

        diffResponse.Value.Nodes.Should().HaveCount(1);
        diffResponse.Value.Nodes[0].Status.Should().Be(DiffStatus.Added);

        diffResponse.Value.Element1ds.Should().HaveCount(1);
        diffResponse.Value.Element1ds[0].Status.Should().Be(DiffStatus.Modified);
        diffResponse.Value.Element1ds[0].Entity.Id.Should().Be(2);
        diffResponse.Value.Element1ds[0].Entity.StartNodeId.Should().Be(2);
        diffResponse.Value.Element1ds[0].Entity.EndNodeId.Should().Be(4);
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
            .Diff.GitModelDiffAsync(diffRequest);

        diffResponse.ThrowIfError();

        diffResponse.Value.Materials.Should().HaveCount(1);
        diffResponse.Value.Materials[0].Status.Should().Be(DiffStatus.Added);
        diffResponse.Value.Materials[0].Entity.Id.Should().Be(2);
    }
}
