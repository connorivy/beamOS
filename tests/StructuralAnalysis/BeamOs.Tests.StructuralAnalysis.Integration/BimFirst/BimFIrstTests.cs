using System.Collections.Concurrent;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Sdk;
using FluentAssertions;

namespace BeamOs.Tests.StructuralAnalysis.Integration.Api;

[MethodDataSource(typeof(ApiClients), nameof(ApiClients.GetClients))]
public class BimFirstTests(ApiClientKey client)
{
    private static readonly SemaphoreSlim semaphore = new(1, 1);
    private static readonly ConcurrentDictionary<ApiClientKey, Guid> ClientModelIds = [];
    private static readonly ConcurrentDictionary<ApiClientKey, Guid> BimFirstSourceModelIds = [];
    private Guid ModelId => ClientModelIds[client];
    private Guid BimSourceModelId => BimFirstSourceModelIds[client];
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

            CreateModelRequest request = new()
            {
                Name = "test model",
                Description = "test model",
                Settings = new()
                {
                    UnitSettings = UnitSettingsContract.K_FT,
                    WorkflowSettings = new() { ModelingMode = ModelingMode.BimFirst },
                },
            };
            var createModelResponse = await this.ApiClient.Models.CreateModelAsync(request);
            createModelResponse.ThrowIfError();
            var modelId = createModelResponse.Value.Id;

            ClientModelIds[client] = modelId;
            var bimSourceModelId =
                createModelResponse.Value.Settings.WorkflowSettings.BimSourceModelId
                ?? throw new InvalidOperationException(
                    "Bim Source Model Id should not be null after creating a Bim First model."
                );
            BimFirstSourceModelIds[client] = bimSourceModelId;
            BeamOsDynamicModel beamOsDynamicModel = new(
                bimSourceModelId,
                new()
                {
                    UnitSettings = UnitSettingsContract.K_FT,
                    WorkflowSettings = new() { ModelingMode = ModelingMode.BimFirst },
                },
                "test model",
                "test model"
            );
            beamOsDynamicModel.AddMaterial(1, 100, 100);
            beamOsDynamicModel.AddSectionProfileFromLibrary(
                1,
                "W12X26",
                StructuralCode.AISC_360_16
            );
            await beamOsDynamicModel.CreateOrUpdate(
                AssemblySetup.StructuralAnalysisRemoteApiClient
            );
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
        BimFirstSourceModelIds[client] =
            modelResponse.Value.Settings.WorkflowSettings.BimSourceModelId
            ?? throw new InvalidOperationException(
                "Bim Source Model Id should not be null after creating a Bim First model."
            );
        var bimSourceModel = await this
            .ApiClient.Models[modelResponse.Value.Settings.WorkflowSettings.BimSourceModelId.Value]
            .GetModelAsync();
        bimSourceModel.ThrowIfError();
    }

    [Test]
    [DependsOn(nameof(BimFirstModel_ShouldHaveCreatedSelfAndBimFirstSourceModel))]
    public async Task PushInitialBimModel_ShouldCreateBimModelAndProposal()
    {
        var modelProposalResponse = await this.ModelClient.Proposals.GetModelProposalsAsync();
        modelProposalResponse.ThrowIfError();
        modelProposalResponse.Value.Should().HaveCount(0);

        var patchModelRequest = new PatchModelRequest()
        {
            Element1dsToAddOrUpdateByExternalId =
            [
                new Element1dByLocationRequest()
                {
                    ExternalId = "Bim-Element-1",
                    StartNodeLocation = new Point(0, 0, 0, LengthUnitContract.Foot),
                    EndNodeLocation = new Point(20, 0, 0, LengthUnitContract.Foot),
                },
                new Element1dByLocationRequest()
                {
                    ExternalId = "Bim-Element-2",
                    StartNodeLocation = new Point(20, 0, 0, LengthUnitContract.Foot),
                    EndNodeLocation = new Point(20, 20, 0, LengthUnitContract.Foot),
                },
                new Element1dByLocationRequest()
                {
                    ExternalId = "Bim-Element-3",
                    StartNodeLocation = new Point(20, 20, 0, LengthUnitContract.Foot),
                    EndNodeLocation = new Point(0, 20, 0, LengthUnitContract.Foot),
                },
                new Element1dByLocationRequest()
                {
                    ExternalId = "Bim-Element-4",
                    StartNodeLocation = new Point(0, 20, 0, LengthUnitContract.Foot),
                    EndNodeLocation = new Point(0, 0, 0, LengthUnitContract.Foot),
                },
            ],
        };
        var patchResponse = await this
            .ApiClient.Models[this.BimSourceModelId]
            .PatchModelAsync(patchModelRequest);
        patchResponse.ThrowIfError();

        var newModelProposalResponse = await this.ModelClient.Proposals.GetModelProposalsAsync();
        newModelProposalResponse.ThrowIfError();
        newModelProposalResponse.Value.Should().HaveCount(1);
    }
}
