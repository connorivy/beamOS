using System.Collections.Concurrent;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Sdk;
using BeamOs.Tests.Common;

namespace BeamOs.Tests.StructuralAnalysis.Integration.Api;

public class DiffModelTests
{
    private static readonly SemaphoreSlim semaphore = new(1, 1);
    private static Guid modelIdA;
    private static Guid modelIdB;

    [Before(HookType.Test)]
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
            .GetModelDiffAsync(diffRequest);

        diffResponse.ThrowIfError();

        diffResponse.Value.Element1ds.Should().BeEmpty();
        diffResponse.Value.Nodes.Should().BeEmpty();
    }

    // [Test]
    // public async Task DiffAfterChanges_ShouldReturnDifferences()
    // {
    //     var patchRequest = new PatchModelRequest()
    //     {
    //         MaterialRequests = [ new CreateMaterialRequest() { Id = 1, ]
    //     };

    //     var patchResponse = await AssemblySetup
    //         .StructuralAnalysisRemoteApiClient.Models[modelIdA]
    //         .PatchModelAsync(patchRequest);
    //     patchResponse.ThrowIfError();

    //     var diffRequest = new DiffModelRequest() { TargetModelId = modelIdB };

    //     var diffResponse = await AssemblySetup
    //         .StructuralAnalysisRemoteApiClient.Models[modelIdA]
    //         .GetDiffedModelAsync(diffRequest);

    //     diffResponse.ThrowIfError();

    //     diffResponse.Value.Element1ds.Should().HaveCount(1);
    //     diffResponse.Value.Nodes.Should().HaveCount(1);
    // }
}
