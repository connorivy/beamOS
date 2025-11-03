using System.Collections.Concurrent;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Sdk;
using FluentAssertions;

namespace BeamOs.Tests.StructuralAnalysis.Integration.Api;

[MethodDataSource(typeof(ApiClients), nameof(ApiClients.GetClients))]
public class BimFirstTests(ApiClientKey client)
{
    private static readonly SemaphoreSlim semaphore = new(1, 1);
    private static readonly ConcurrentDictionary<ApiClientKey, Guid> ClientModelIds = [];
    private static readonly ConcurrentDictionary<
        ApiClientKey,
        ApiResponse<ModelResponse>
    > ModelResponses = [];
    private Guid ModelId => ClientModelIds[client];
    private ApiResponse<ModelResponse> ModelResponseResult => ModelResponses[client];
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

            CreateModelRequest request = new()
            {
                Name = "test model",
                Description = "test model",
                Settings = new()
                {
                    UnitSettings = UnitSettingsContract.K_FT,
                    WorkflowSettings = new() { ModelingMode = ModelingMode.BimFirst },
                },
                // ModelingMode = ModelingModeContract.BimFirst,
                Id = modelId,
            };

            ClientModelIds[client] = modelId;
            ModelResponses[client] = await this.ApiClient.Models.CreateModelAsync(request);
        }
        finally
        {
            semaphore.Release();
        }
    }

    [Test]
    public async Task BimFirstModel_ShouldHaveCreatedSelfAndBimFirstSourceModel()
    {
        var modelResponse = await this.ApiClient.Models[this.ModelId].GetModelAsync();
        modelResponse.ThrowIfError();

        modelResponse.Value.Settings.WorkflowSettings.BimSourceModelId.Should().NotBeNull();
        var bimSourceModel = await this
            .ApiClient.Models[modelResponse.Value.Settings.WorkflowSettings.BimSourceModelId.Value]
            .GetModelAsync();
        bimSourceModel.ThrowIfError();
    }

    // [Test]
    // public async Task PushInitialBimModel_ShouldCreateBimModelAndProposal()
    // {
    //     var modelProposalResponse = await this.ModelClient.Proposals.GetModelProposalsAsync();
    //     modelProposalResponse.ThrowIfError();
    //     modelProposalResponse.Value.Should().HaveCount(0);

    //     var modelBuilder = new ModelOperations(this.ModelId);

    //     modelBuilder.AddTrackedElement1d(
    //         "Element-1",
    //         new(0, 0, 0, LengthUnitContract.Foot),
    //         new(10, 0, 0, LengthUnitContract.Foot),
    //         null,
    //         null
    //     );
    //     modelBuilder.AddTrackedElement1d(
    //         "Element-2",
    //         new(10, 0, 0, LengthUnitContract.Foot),
    //         new(10, 10, 0, LengthUnitContract.Foot),
    //         null,
    //         null
    //     );
    //     modelBuilder.AddTrackedElement1d(
    //         "Element-3",
    //         new(10, 10, 0, LengthUnitContract.Foot),
    //         new(0, 10, 0, LengthUnitContract.Foot),
    //         null,
    //         null
    //     );

    //     await modelBuilder.PushChanges(this.ModelClient);

    //     var newModelProposalResponse = await this.ModelClient.Proposals.GetModelProposalsAsync();
    //     newModelProposalResponse.ThrowIfError();
    //     newModelProposalResponse.Value.Should().HaveCount(1);
    // }
}
