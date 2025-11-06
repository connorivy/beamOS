using System.Collections.Concurrent;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Sdk;
using FluentAssertions;

namespace BeamOs.Tests.StructuralAnalysis.Integration.Api;

[MethodDataSource(typeof(ApiClients), nameof(ApiClients.GetClients))]
public class PatchModelTests(ApiClientKey client)
{
    private static readonly SemaphoreSlim semaphore = new(1, 1);
    private static readonly ConcurrentDictionary<ApiClientKey, Guid> ClientModelIds = [];
    private Guid ModelId => ClientModelIds[client];
    private BeamOsResultApiClient ApiClient => client.GetClient();
    private BeamOsApiResultModelId ModelClient => this.ApiClient.Models[this.ModelId];

    [Before(HookType.Test)]
    public async Task SetupModel()
    {
        await semaphore.WaitAsync();
        try
        {
            if (ClientModelIds.ContainsKey(client))
            {
                return;
            }

            var modelId = Guid.NewGuid();

            BeamOsDynamicModel model = new(
                modelId,
                new(UnitSettingsContract.K_IN),
                "test model",
                "test model"
            );
            model.AddSectionProfileFromLibrary(1, "W10X33", StructuralCode.AISC_360_16);
            model.AddMaterial(1, 29000, 11500);

            await model.CreateOnly(this.ApiClient);

            ClientModelIds[client] = modelId;
        }
        finally
        {
            semaphore.Release();
        }
    }

    [Test]
    public async Task CreateElement1dByLocation_ShouldReturnSuccessfulResponse()
    {
        var patchRequest = new PatchModelRequest()
        {
            Element1dsToAddOrUpdateByExternalId =
            [
                new Element1dByLocationRequest()
                {
                    ExternalId = "element-1",
                    StartNodeLocation = new Point(0, 0, 0, LengthUnitContract.Meter),
                    EndNodeLocation = new Point(10, 0, 0, LengthUnitContract.Meter),
                },
            ],
        };

        var patchResponse = await this.ModelClient.PatchModelAsync(patchRequest);
        patchResponse.ThrowIfError();

        var modelResponse = await this.ModelClient.GetModelAsync();
        modelResponse.ThrowIfError();

        modelResponse.Value.Element1ds.Should().HaveCount(1);
        modelResponse.Value.Nodes.Should().HaveCount(2);
    }

    [Test]
    [DependsOn(nameof(CreateElement1dByLocation_ShouldReturnSuccessfulResponse))]
    public async Task CreateElement1dByLocation_ShouldReuseExistingNodes()
    {
        var patchRequest = new PatchModelRequest()
        {
            Element1dsToAddOrUpdateByExternalId =
            [
                new Element1dByLocationRequest()
                {
                    ExternalId = "element-1",
                    StartNodeLocation = new Point(0, 0, 0, LengthUnitContract.Meter),
                    EndNodeLocation = new Point(0, 10, 0, LengthUnitContract.Meter),
                },
                new Element1dByLocationRequest()
                {
                    ExternalId = "element-2",
                    StartNodeLocation = new Point(0, 0, 0, LengthUnitContract.Meter),
                    EndNodeLocation = new Point(10, 0, 0, LengthUnitContract.Meter),
                },
            ],
        };

        var patchResponse = await this.ModelClient.PatchModelAsync(patchRequest);
        patchResponse.ThrowIfError();

        var modelResponse = await this.ModelClient.GetModelAsync();
        modelResponse.ThrowIfError();

        modelResponse.Value.Element1ds.Should().HaveCount(2);
        modelResponse.Value.Nodes.Should().HaveCount(3);
    }
}
