using System.Collections.Concurrent;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Application.Common;
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

    [Test]
    [DependsOn(nameof(PushInitialBimModel_ShouldCreateBimModelAndProposal))]
    public async Task ReplaceSourceModel_ShouldTrackDiffsAndPushChangeProposal()
    {
        // Step 1: Simulate a Revit update by pushing a new set of elements to the source model
        var newPatchModelRequest = new PutModelRequest()
        {
            Element1dsToAddOrUpdateByExternalId =
            [
                // New geometry: one element changed, one added, one removed
                new Element1dByLocationRequest()
                {
                    ExternalId = "Bim-Element-1", // Changed location
                    StartNodeLocation = new Point(-10, 7, 17, LengthUnitContract.Foot),
                    EndNodeLocation = new Point(22, -5, 12, LengthUnitContract.Foot),
                },
                new Element1dByLocationRequest()
                {
                    ExternalId = "Bim-Element-2", // Unchanged
                    StartNodeLocation = new Point(20, 0, 0, LengthUnitContract.Foot),
                    EndNodeLocation = new Point(20, 20, 0, LengthUnitContract.Foot),
                },
                new Element1dByLocationRequest()
                {
                    ExternalId = "Bim-Element-3", // Unchanged
                    StartNodeLocation = new Point(20, 20, 0, LengthUnitContract.Foot),
                    EndNodeLocation = new Point(0, 20, 0, LengthUnitContract.Foot),
                },
                // new Element1dByLocationRequest()
                // {
                //     ExternalId = "Bim-Element-4", // Removed
                //     StartNodeLocation = new Point(0, 20, 0, LengthUnitContract.Foot),
                //     EndNodeLocation = new Point(0, 0, 0, LengthUnitContract.Foot),
                // },
                new Element1dByLocationRequest()
                {
                    ExternalId = "Bim-Element-5", // New element
                    StartNodeLocation = new Point(10, 10, 0, LengthUnitContract.Foot),
                    EndNodeLocation = new Point(30, 10, 0, LengthUnitContract.Foot),
                },
            ],
        };
        var patchResponse = await this
            .ApiClient.Models[this.BimSourceModelId]
            .Source.PutSourceModelAsync(newPatchModelRequest);
        patchResponse.ThrowIfError();

        // Step 2: Assert the source model is replaced (new elements present, old ones removed/updated)
        var modelResponse = await this.ApiClient.Models[this.BimSourceModelId].GetModelAsync();
        modelResponse.ThrowIfError();
        var elementIds = modelResponse.Value.Element1ds.Select(e => e.ExternalId).ToList();
        elementIds.Should().Contain("Bim-Element-1");
        elementIds.Should().Contain("Bim-Element-2");
        elementIds.Should().Contain("Bim-Element-3");
        elementIds.Should().Contain("Bim-Element-5");
        elementIds.Should().NotContain("Bim-Element-4");

        // assert new location for bim-element-1
        var element1 = modelResponse.Value.Element1ds.First(e => e.ExternalId == "Bim-Element-1");
        var startLocation = modelResponse
            .Value.Nodes.First(n => n.Id == element1.StartNodeId)
            .LocationPoint.ToDomain();
        startLocation.X.Feet.Should().BeApproximately(-10, 0.001);
        startLocation.Y.Feet.Should().BeApproximately(7, 0.001);
        startLocation.Z.Feet.Should().BeApproximately(17, 0.001);

        var endLocation = modelResponse
            .Value.Nodes.First(n => n.Id == element1.EndNodeId)
            .LocationPoint.ToDomain();
        endLocation.X.Feet.Should().BeApproximately(22, 0.001);
        endLocation.Y.Feet.Should().BeApproximately(-5, 0.001);
        endLocation.Z.Feet.Should().BeApproximately(12, 0.001);

        // Step 3: Assert that the system tracks the differences between old and new source models
        // (This would typically be exposed via a diff API or proposal)
        var proposalsResponse = await this.ModelClient.Proposals.GetModelProposalsAsync();
        proposalsResponse.ThrowIfError();
        proposalsResponse
            .Value.Should()
            .HaveCount(2, "A new proposal should be created for the diff");
    }
}
