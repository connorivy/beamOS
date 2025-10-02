using System.Collections.Concurrent;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Sdk;
using FluentAssertions;

namespace BeamOs.Tests.StructuralAnalysis.Integration.Api;

[MethodDataSource(typeof(ApiClients), nameof(ApiClients.GetClients))]
public class EndToEndTests(ApiClientKey client)
{
    private static readonly ConcurrentDictionary<ApiClientKey, Guid> ClientModelIds = [];
    private static readonly ConcurrentDictionary<
        ApiClientKey,
        ApiResponse<ModelResponse>
    > ModelResponses = [];
    private Guid ModelId => ClientModelIds[client];
    private ApiResponse<ModelResponse> ModelResponseResult
    {
        get
        {
            if (!ModelResponses.TryGetValue(client, out var response))
            {
                var availableKeys = string.Join(", ", ModelResponses.Keys.Select(k => k.Key));
                throw new KeyNotFoundException(
                    $"Key '{client.Key}' not found in ModelResponses. Available keys: [{availableKeys}]. Did SetupModel run?"
                );
            }
            return response;
        }
    }
    private BeamOsResultApiClient ApiClient => client.GetClient();
    private BeamOsApiResultModelId ModelClient => this.ApiClient.Models[this.ModelId];

    [Before(HookType.Test)]
    public void SetupModel()
    {
        Console.WriteLine($"Setting up model for client: {client.Key}");
        if (ClientModelIds.ContainsKey(client))
        {
            Console.WriteLine($"Model already set up for client: {client.Key}");
            return;
        }

        var modelId = Guid.NewGuid();

        CreateModelRequest request = new()
        {
            Name = "test model",
            Description = "test model",
            Settings = new(UnitSettingsContract.K_FT),
            Id = modelId,
        };

        ClientModelIds[client] = modelId;
        ModelResponses[client] = this
            .ApiClient.Models.CreateModelAsync(request)
            .GetAwaiter()
            .GetResult();
    }

    [Test]
    public async Task CreateModel_ShouldReturnSuccessfulResponse()
    {
        Console.WriteLine(
            $"Running CreateModel_ShouldReturnSuccessfulResponse for client: {client.Key}"
        );
        await Verify(this.ModelResponseResult);
    }

    [Test]
    public async Task CreateModelWithDuplicateId_ShouldReturnConflictError()
    {
        CreateModelRequest request = new()
        {
            Name = "test model",
            Description = "test model",
            Settings = new(UnitSettingsContract.K_FT),
            Id = this.ModelId,
        };

        var modelResponseResult = await this.ApiClient.Models.CreateModelAsync(request);

        await Verify(modelResponseResult).ScrubInlineGuids();
    }

    [Test]
    public async Task CreateNode_WithNoSpecifiedId_ShouldCreateNode_AndGiveAnId()
    {
        CreateNodeRequest createNodeRequestBody = new(
            new(1, 1, 1, LengthUnitContract.Foot),
            Restraint.Fixed
        );

        var nodeResponseResult = await this.ModelClient.Nodes.CreateNodeAsync(
            createNodeRequestBody
        );

        await Verify(nodeResponseResult)
            .ScrubMembers(l =>
                typeof(IHasIntId).IsAssignableFrom(l.DeclaringType) && l.Name == "Id"
            );
    }

    [Test]
    public async Task CreateLoadCase_ShouldCreateLoadCase()
    {
        LoadCaseData data = new() { Name = "Dead" };

        var loadCaseResponse = await this.ModelClient.LoadCases.CreateLoadCaseAsync(data);

        await Verify(loadCaseResponse);
    }

    [Test]
    public async Task CreateLoadCombination_ShouldCreateLoadCombination()
    {
        LoadCombinationData data = new((1, 1.0));

        var loadCombinationResponse =
            await this.ModelClient.LoadCombinations.CreateLoadCombinationAsync(data);

        await Verify(loadCombinationResponse);
    }

    [Test]
    [DependsOn(nameof(CreateNode_WithNoSpecifiedId_ShouldCreateNode_AndGiveAnId))]
    public async Task CreateAnotherNode_WithDifferentModelId_ShouldAssignNodeIdOf1()
    {
        CreateModelRequest request = new()
        {
            Name = "another test model",
            Description = "test model",
            Settings = new(UnitSettingsContract.K_FT),
            Id = Guid.NewGuid(),
        };

        var modelResponse = await this.ApiClient.Models.CreateModelAsync(request);

        modelResponse.IsSuccess.Should().BeTrue();

        CreateNodeRequest createNodeRequestBody = new(
            new(1, 1, 1, LengthUnitContract.Foot),
            Restraint.Fixed
        );
        var newModelClient = this.ApiClient.Models[modelResponse.Value.Id];

        var nodeResponseResult = await newModelClient.Nodes.CreateNodeAsync(createNodeRequestBody);

        nodeResponseResult.Value.Id.Should().BeLessThan(5);
    }

    [Test]
    [DependsOn(nameof(CreateNode_WithNoSpecifiedId_ShouldCreateNode_AndGiveAnId))]
    public async Task CreateNode_WithIdOf5_ShouldCreateNode_WithCorrectId()
    {
        CreateNodeRequest createNodeRequestBody = new(
            new(1, 1, 1, LengthUnitContract.Foot),
            Restraint.Fixed,
            5
        );

        var nodeResponseResult = await this.ModelClient.Nodes.CreateNodeAsync(
            createNodeRequestBody
        );

        await Verify(nodeResponseResult);
    }

    [Test]
    [DependsOn(nameof(CreateNode_WithIdOf5_ShouldCreateNode_WithCorrectId))]
    [DependsOn(nameof(CreateLoadCase_ShouldCreateLoadCase))]
    public async Task CreatePointLoad_ShouldCreatePointLoad()
    {
        CreatePointLoadRequest requestBody = new()
        {
            NodeId = 5,
            LoadCaseId = 1,
            Direction = new(0, -1, 0),
            Force = new(10, ForceUnitContract.KilopoundForce),
            Id = 5,
        };

        var result = await this.ModelClient.PointLoads.CreatePointLoadAsync(requestBody);

        await Verify(result);
    }

    [Test]
    [DependsOn(nameof(CreateNode_WithIdOf5_ShouldCreateNode_WithCorrectId))]
    [DependsOn(nameof(CreateLoadCase_ShouldCreateLoadCase))]
    public async Task CreateMomentLoad_ShouldCreateMomentLoad()
    {
        CreateMomentLoadRequest requestBody = new()
        {
            NodeId = 5,
            LoadCaseId = 1,
            AxisDirection = new(0, 0, 1),
            Torque = new(11.7, TorqueUnitContract.KilopoundForceFoot),
            Id = 5,
        };

        var result = await this.ModelClient.MomentLoads.CreateMomentLoadAsync(requestBody);

        await Verify(result);
    }

    [Test]
    public async Task CreateSectionProfile_WithSpecifiedId_ShouldCreateSectionProfile_WithCorrectId()
    {
        var settings = new VerifySettings();
        settings.ScrubLinesWithReplace(replaceLine: _ =>
        {
            if (_.Contains("LineE"))
            {
                return "NoMoreLineE";
            }

            return _;
        });
        CreateSectionProfileRequest w16x36Request = new()
        {
            Name = "W16x36",
            Area = 10.6,
            StrongAxisMomentOfInertia = 448,
            WeakAxisMomentOfInertia = 24.5,
            PolarMomentOfInertia = .55,
            StrongAxisShearArea = 5.0095,
            WeakAxisShearArea = 4.6905,
            StrongAxisPlasticSectionModulus = 56.4,
            WeakAxisPlasticSectionModulus = 3.4,
            LengthUnit = LengthUnitContract.Inch,
            Id = 1636,
        };
        var sectionProfileResponseResult =
            await this.ModelClient.SectionProfiles.CreateSectionProfileAsync(w16x36Request);

        await Verify(sectionProfileResponseResult);
    }

    [Test]
    public async Task CreateSectionProfile_FromAiscLibrary_ShouldCreateSectionProfile()
    {
        SectionProfileFromLibraryData w14x22 = new()
        {
            Name = "W14x22",
            Library = StructuralCode.AISC_360_16,
        };
        var sectionProfileResponseResult =
            await this.ModelClient.SectionProfiles.FromLibrary.AddSectionProfileFromLibraryAsync(
                w14x22
            );

        await Verify(sectionProfileResponseResult)
            .ScrubMembers(l =>
                typeof(IHasIntId).IsAssignableFrom(l.DeclaringType) && l.Name == "Id"
            );
    }

    [Test]
    public async Task CreateMaterial_WithSpecifiedId_ShouldCreateMaterial_WithCorrectId()
    {
        CreateMaterialRequest a992Request = new()
        {
            ModulusOfElasticity = 29000,
            ModulusOfRigidity = 11_153.85,
            PressureUnit = PressureUnitContract.KilopoundForcePerSquareInch,
            Id = 992,
        };

        var materialResponseResult = await this.ModelClient.Materials.CreateMaterialAsync(
            a992Request
        );

        await Verify(materialResponseResult);
    }

    [Test]
    [DependsOn(nameof(CreateNode_WithIdOf5_ShouldCreateNode_WithCorrectId))]
    [DependsOn(nameof(CreateMaterial_WithSpecifiedId_ShouldCreateMaterial_WithCorrectId))]
    [DependsOn(
        nameof(CreateSectionProfile_WithSpecifiedId_ShouldCreateSectionProfile_WithCorrectId)
    )]
    public async Task CreateElement1d_ShouldCreateElement1d()
    {
        // create another node with id = 6
        CreateNodeRequest createNodeRequestBody = new(
            new(1, 1, 1, LengthUnitContract.Foot),
            Restraint.Fixed,
            6
        );

        var nodeResponseResult = await this.ModelClient.Nodes.CreateNodeAsync(
            createNodeRequestBody
        );

        await Assert.That(nodeResponseResult.IsSuccess).IsTrue();

        CreateElement1dRequest elRequest = new()
        {
            StartNodeId = 5,
            EndNodeId = 6,
            MaterialId = 992,
            SectionProfileId = 1636,
            Id = 99,
        };

        var elResponseResult = await this.ModelClient.Element1ds.CreateElement1dAsync(elRequest);

        await Verify(elResponseResult);
    }

    [Test]
    [DependsOn(nameof(CreateElement1d_ShouldCreateElement1d))]
    public async Task CreateInternalNode_ShouldCreateInternalNode()
    {
        CreateInternalNodeRequest requestBody = new(
            99,
            new(50, RatioUnitContract.Percent),
            null,
            10
        );

        var internalNodeResponseResult =
            await this.ModelClient.Nodes.Internal.CreateInternalNodeAsync(requestBody);

        await Verify(internalNodeResponseResult)
            .ScrubMembers(l =>
                typeof(IHasIntId).IsAssignableFrom(l.DeclaringType) && l.Name == "Id"
            );
    }

    [Test]
    [DependsOn(nameof(CreateInternalNode_ShouldCreateInternalNode))]
    public async Task ChangeInternalNodeToSpatial_ShouldChangeNode()
    {
        var x = await this
            .ModelClient.Nodes[10]
            .PutNodeAsync(
                new NodeData()
                {
                    LocationPoint = new(50, 50, 50, LengthUnitContract.Meter),
                    Restraint = Restraint.PinnedXyPlane,
                }
            );

        await Verify(x)
            .ScrubMembers(l =>
                typeof(IHasIntId).IsAssignableFrom(l.DeclaringType) && l.Name == "Id"
            );
    }

    [Test]
    [DependsOn(nameof(CreateElement1d_ShouldCreateElement1d))]
    public async Task GetElement1d_ShouldResultInExpectedResponse()
    {
        var elResponseResult = await this.ModelClient.Element1ds[99].GetElement1dAsync();

        await Verify(elResponseResult);
    }

    [Test]
    [DependsOn(nameof(CreateNode_WithIdOf5_ShouldCreateNode_WithCorrectId))]
    public async Task UpdateNode_WithPartialLocation_ShouldPartiallyUpdate()
    {
        UpdateNodeRequest updateNodeRequest = new(
            5,
            new() { LengthUnit = LengthUnitContract.Meter, X = 50 },
            Restraint.Free
        );

        var nodeResponseResult = await this.ModelClient.Nodes.PatchNodeAsync(updateNodeRequest);

        await Verify(nodeResponseResult);
    }

    [Test]
    [DependsOn(nameof(UpdateNode_WithPartialLocation_ShouldPartiallyUpdate))]
    [DependsOn(nameof(CreateElement1d_ShouldCreateElement1d))]
    public async Task ChangeSpatialNodeToInternal_ShouldChangeNode()
    {
        var changeToInternal = await this
            .ModelClient.Nodes[5]
            .Internal.PutInternalNodeAsync(
                new InternalNodeData(99, new(50, RatioUnitContract.Percent))
            );
        changeToInternal.ThrowIfError();

        // element with id 99 has start node with id 5
        // changing the node type should not affect the element
        var existingElement = await this.ModelClient.Element1ds[99].GetElement1dAsync();

        var internalNodeResponseResult = await this
            .ModelClient.Nodes[5]
            .Internal.GetInternalNodeAsync();

        await Verify(internalNodeResponseResult);
    }

    // [Test]
    // [DependsOn(nameof(GetElement1d_ShouldResultInExpectedResponse))]
    // public async Task CreateModelProposal_ShouldCreateModelProposal()
    // {
    //     var modelProposalRequest = new ModelProposalData
    //     {
    //         Name = "a new name!!!",
    //         Description = "a new description!!!",
    //         CreateNodeProposals =
    //         [
    //             new()
    //             {
    //                 Id = 1,
    //                 LocationPoint = new(2, 2, 2, LengthUnitContract.Foot),
    //                 Restraint = Restraint.Fixed,
    //             },
    //         ],
    //         CreateElement1dProposals =
    //         [
    //             Element1dProposalBase.Create(
    //                 ProposedID.Existing(5),
    //                 ProposedID.Proposed(1),
    //                 ProposedID.Existing(992),
    //                 ProposedID.Existing(1636)
    //             ),
    //         ],
    //         ModifyElement1dProposals =
    //         [
    //             new ModifyElement1dProposal()
    //             {
    //                 ExistingElement1dId = 99,
    //                 EndNodeId = ProposedID.Proposed(1),
    //             },
    //         ],
    //     };

    //     var modelProposalResponseResult = await modelClient.Proposals.CreateModelProposalAsync(
    //         modelProposalRequest
    //     );

    //     await Verify(modelProposalResponseResult)
    //         .ScrubMembers(l =>
    //             typeof(IHasIntId).IsAssignableFrom(l.DeclaringType) && l.Name == "Id"
    //         );
    // }

    [Test]
    [DependsOn(nameof(CreateMaterial_WithSpecifiedId_ShouldCreateMaterial_WithCorrectId))]
    [DependsOn(
        nameof(CreateSectionProfile_WithSpecifiedId_ShouldCreateSectionProfile_WithCorrectId)
    )]
    public async Task DeleteNode_ShouldAlsoDeleteDependentElement1ds()
    {
        var newNode1Id = 456;
        var newNode2Id = 789;
        var newElement1dId = 1234;
        var newNode1Response = await this.ModelClient.Nodes.CreateNodeAsync(
            new CreateNodeRequest(
                new(7, 7, 7, LengthUnitContract.Foot),
                Restraint.Fixed,
                newNode1Id
            )
        );
        var newNode2Response = await this.ModelClient.Nodes.CreateNodeAsync(
            new CreateNodeRequest(
                new(17, 17, 17, LengthUnitContract.Foot),
                Restraint.Fixed,
                newNode2Id
            )
        );
        newNode1Response.ThrowIfError();
        newNode2Response.ThrowIfError();

        var newElement1dResponse = await this.ModelClient.Element1ds.CreateElement1dAsync(
            new CreateElement1dRequest()
            {
                Id = newElement1dId,
                StartNodeId = newNode1Id,
                EndNodeId = newNode2Id,
                MaterialId = 992,
                SectionProfileId = 1636,
            }
        );
        newElement1dResponse.ThrowIfError();

        if (
            newElement1dResponse.Value.StartNodeId != newNode1Id
            || newElement1dResponse.Value.EndNodeId != newNode2Id
        )
        {
            throw new InvalidOperationException(
                "The newly created element1d does not have the expected start and end node IDs."
            );
        }

        var deleteNodeResponse = await this.ModelClient.Nodes[newNode1Id].DeleteNodeAsync();
        deleteNodeResponse.ThrowIfError();

        // Verify that the element1d with id 1234 has been deleted
        var getElement1dResponse = await this.ModelClient.Element1ds[1234].GetElement1dAsync();

        await Verify(getElement1dResponse)
            .ScrubInlineGuids()
            .ScrubMembers(l =>
                typeof(IHasIntId).IsAssignableFrom(l.DeclaringType) && l.Name == "Id"
            );
    }
}

public class IntegrationTestContext
{
    public required ApiClientKey ClientKey { get; init; }
    public Guid ModelId { get; init; }
}
