using System.Collections.Concurrent;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Sdk;
using FluentAssertions;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

[MethodDataSource(typeof(ApiClients), nameof(ApiClients.GetClients))]
public class OctreeTests(ApiClientKey client)
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
    public async Task AddNodeAtDuplicatePosition_ShouldResultInError()
    {
        var modelResponse = this.ModelResponseResult;
        modelResponse.ThrowIfError();

        var modelClient = this.ApiClient.Models[this.ModelId];
        CreateNodeRequest createNodeRequest = new(
            new(1, 2, 3, LengthUnitContract.Foot),
            Restraint.Free
        );

        var nodeResponse = await modelClient.Nodes.CreateNodeAsync(createNodeRequest);
        nodeResponse.ThrowIfError();

        var duplicateNodeResponse = await modelClient.Nodes.CreateNodeAsync(createNodeRequest);
        duplicateNodeResponse.IsSuccess.Should().BeFalse();
        // await Verify(duplicateNodeResponse).ScrubInlineGuids();
    }
}
