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
    private static readonly SemaphoreSlim semaphore = new(1, 1);
    private static Guid modelIdA;
    private static Guid modelIdB;

    [Before(TUnit.Core.HookType.Test)]
    public async Task SetupModel()
    {
        await semaphore.WaitAsync();
        try
        {
            if (modelIdA != default)
            {
                return;
            }

            modelIdA = Guid.NewGuid();
            modelIdB = Guid.NewGuid();

            BeamOsDynamicModel modelA = new(
                modelIdA,
                new() { UnitSettings = UnitSettingsContract.K_FT },
                "test model",
                "test model"
            );
            BeamOsDynamicModel modelB = new(
                modelIdB,
                new() { UnitSettings = UnitSettingsContract.K_FT },
                "test model",
                "test model"
            );
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
        }
        finally
        {
            semaphore.Release();
        }
    }

    [Test]
    public async Task InitialDiff_ShouldReturnEmptyDifferences()
    {
        var diffRequest = new DiffModelRequest() { TargetModelId = modelIdB };

        var diffResponse = await AssemblySetup
            .StructuralAnalysisRemoteApiClient.Models[modelIdA]
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
        // Add a node to model B
        var createNodeResponse = await AssemblySetup
            .StructuralAnalysisRemoteApiClient.Models[modelIdB]
            .Nodes.CreateNodeAsync(
                new CreateNodeRequest()
                {
                    Id = 4,
                    LocationPoint = new()
                    {
                        X = 0,
                        Y = 0,
                        Z = 30,
                        LengthUnit = BeamOs.StructuralAnalysis.Contracts.Common.LengthUnit.Foot,
                    },
                    Restraint = Restraint.Free,
                }
            );
        createNodeResponse.ThrowIfError();

        var diffRequest = new DiffModelRequest() { TargetModelId = modelIdB };

        var diffResponse = await AssemblySetup
            .StructuralAnalysisRemoteApiClient.Models[modelIdA]
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
        // Remove an element from model B
        var deleteResponse = await AssemblySetup
            .StructuralAnalysisRemoteApiClient.Models[modelIdB]
            .Element1ds[2]
            .DeleteElement1dAsync();
        deleteResponse.ThrowIfError();

        var diffRequest = new DiffModelRequest() { TargetModelId = modelIdB };

        var diffResponse = await AssemblySetup
            .StructuralAnalysisRemoteApiClient.Models[modelIdA]
            .Diff.GitModelDiffAsync(diffRequest);

        diffResponse.ThrowIfError();

        diffResponse.Value.Element1ds.Should().HaveCount(1);
        diffResponse.Value.Element1ds[0].Status.Should().Be(DiffStatus.Removed);
        diffResponse.Value.Element1ds[0].Entity.Id.Should().Be(2);
        diffResponse.Value.Nodes.Should().BeEmpty();
    }

    [Test]
    public async Task DiffWithAddedMaterial_ShouldDetectAddedMaterial()
    {
        // Add a material to model B
        var createMaterialResponse = await AssemblySetup
            .StructuralAnalysisRemoteApiClient.Models[modelIdB]
            .Materials.CreateMaterialAsync(
                new CreateMaterialRequest()
                {
                    Id = 2,
                    ModulusOfElasticity = 200000,
                    ModulusOfRigidity = 80000,
                    PressureUnit = BeamOs.StructuralAnalysis.Contracts.Common.PressureUnit.KilopoundForcePerSquareFoot,
                }
            );
        createMaterialResponse.ThrowIfError();

        var diffRequest = new DiffModelRequest() { TargetModelId = modelIdB };

        var diffResponse = await AssemblySetup
            .StructuralAnalysisRemoteApiClient.Models[modelIdA]
            .Diff.GitModelDiffAsync(diffRequest);

        diffResponse.ThrowIfError();

        diffResponse.Value.Materials.Should().HaveCount(1);
        diffResponse.Value.Materials[0].Status.Should().Be(DiffStatus.Added);
        diffResponse.Value.Materials[0].Entity.Id.Should().Be(2);
    }
}
