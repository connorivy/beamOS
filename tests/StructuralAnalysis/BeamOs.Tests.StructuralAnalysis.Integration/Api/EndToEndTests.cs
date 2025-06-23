using BeamOs.Common.Contracts;
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
using FluentAssertions;

namespace BeamOs.Tests.StructuralAnalysis.Integration.Api;

public class EndToEndTests
{
    private static Guid modelId;
    private static Result<ModelResponse> modelResponseResult;

    [Before(Class)]
    public static async Task SetupModel()
    {
        modelId = Guid.NewGuid();

        CreateModelRequest request = new()
        {
            Name = "test model",
            Description = "test model",
            Settings = new(UnitSettingsContract.K_FT),
            Id = modelId,
        };

        modelResponseResult = await AssemblySetup.StructuralAnalysisApiClient.CreateModelAsync(
            request
        );
    }

    [Test]
    public async Task CreateModel_ShouldReturnSuccessfulResponse()
    {
        await Verify(modelResponseResult);
    }

    [Test]
    public async Task CreateNode_WithNoSpecifiedId_ShouldCreateNode_AndGiveAnId()
    {
        CreateNodeRequest createNodeRequestBody = new(
            new(1, 1, 1, LengthUnitContract.Foot),
            Restraint.Fixed
        );

        var nodeResponseResult = await AssemblySetup.StructuralAnalysisApiClient.CreateNodeAsync(
            modelId,
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

        var loadCaseResponse = await AssemblySetup.StructuralAnalysisApiClient.CreateLoadCaseAsync(
            modelId,
            data
        );

        await Verify(loadCaseResponse);
    }

    [Test]
    public async Task CreateLoadCombination_ShouldCreateLoadCombination()
    {
        LoadCombinationData data = new((1, 1.0));

        var loadCombinationResponse =
            await AssemblySetup.StructuralAnalysisApiClient.CreateLoadCombinationAsync(
                modelId,
                data
            );

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

        var modelResponse = await AssemblySetup.StructuralAnalysisApiClient.CreateModelAsync(
            request
        );

        modelResponse.IsSuccess.Should().BeTrue();

        CreateNodeRequest createNodeRequestBody = new(
            new(1, 1, 1, LengthUnitContract.Foot),
            Restraint.Fixed
        );

        var nodeResponseResult = await AssemblySetup.StructuralAnalysisApiClient.CreateNodeAsync(
            modelResponse.Value.Id,
            createNodeRequestBody
        );

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

        var nodeResponseResult = await AssemblySetup.StructuralAnalysisApiClient.CreateNodeAsync(
            modelId,
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

        var result = await AssemblySetup.StructuralAnalysisApiClient.CreatePointLoadAsync(
            modelId,
            requestBody
        );

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

        var result = await AssemblySetup.StructuralAnalysisApiClient.CreateMomentLoadAsync(
            modelId,
            requestBody
        );

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
            await AssemblySetup.StructuralAnalysisApiClient.CreateSectionProfileAsync(
                modelId,
                w16x36Request
            );

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
            await AssemblySetup.StructuralAnalysisApiClient.AddSectionProfileFromLibraryAsync(
                modelId,
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

        var materialResponseResult =
            await AssemblySetup.StructuralAnalysisApiClient.CreateMaterialAsync(
                modelId,
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

        var nodeResponseResult = await AssemblySetup.StructuralAnalysisApiClient.CreateNodeAsync(
            modelId,
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

        var elResponseResult = await AssemblySetup.StructuralAnalysisApiClient.CreateElement1dAsync(
            modelId,
            elRequest
        );

        await Verify(elResponseResult);
    }

    [Test]
    [DependsOn(nameof(CreateElement1d_ShouldCreateElement1d))]
    public async Task CreateInternalNode_ShouldCreateInternalNode()
    {
        CreateInternalNodeRequest requestBody = new(99, new(50, RatioUnit.Percent), null, 10);

        var internalNodeResponseResult =
            await AssemblySetup.StructuralAnalysisApiClient.CreateInternalNodeAsync(
                modelId,
                requestBody
            );

        await Verify(internalNodeResponseResult)
            .ScrubMembers(l =>
                typeof(IHasIntId).IsAssignableFrom(l.DeclaringType) && l.Name == "Id"
            );
    }

    [Test]
    [DependsOn(nameof(CreateInternalNode_ShouldCreateInternalNode))]
    public async Task ChangeInternalNodeToSpatial_ShouldChangeNode()
    {
        var x = await AssemblySetup.StructuralAnalysisApiClient.PutNodeAsync(
            10,
            modelId,
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
        var elResponseResult = await AssemblySetup.StructuralAnalysisApiClient.GetElement1dAsync(
            modelId,
            99
        );

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

        var nodeResponseResult = await AssemblySetup.StructuralAnalysisApiClient.PatchNodeAsync(
            modelId,
            updateNodeRequest
        );

        await Verify(nodeResponseResult);
    }

    [Test]
    [DependsOn(nameof(UpdateNode_WithPartialLocation_ShouldPartiallyUpdate))]
    [DependsOn(nameof(CreateElement1d_ShouldCreateElement1d))]
    public async Task ChangeSpatialNodeToInternal_ShouldChangeNode()
    {
        var changeToInternal = await AssemblySetup.StructuralAnalysisApiClient.PutInternalNodeAsync(
            modelId,
            5,
            new InternalNodeData(99, new(50, RatioUnit.Percent))
        );
        changeToInternal.ThrowIfError();

        // element with id 99 has start node with id 5
        // changing the node type should not affect the element
        var existingElement = await AssemblySetup.StructuralAnalysisApiClient.GetElement1dAsync(
            modelId,
            99
        );

        var internalNodeResponseResult =
            await AssemblySetup.StructuralAnalysisApiClient.GetInternalNodeAsync(modelId, 5);

        await Verify(internalNodeResponseResult);
    }

    [Test]
    [DependsOn(nameof(GetElement1d_ShouldResultInExpectedResponse))]
    public async Task CreateModelProposal_ShouldCreateModelProposal()
    {
        var modelProposalRequest = new ModelProposalData
        {
            Name = "a new name!!!",
            Description = "a new description!!!",
            CreateNodeProposals =
            [
                new()
                {
                    Id = 1,
                    LocationPoint = new(2, 2, 2, LengthUnitContract.Foot),
                    Restraint = Restraint.Fixed,
                },
            ],
            CreateElement1dProposals =
            [
                Element1dProposalBase.Create(
                    ProposedID.Existing(5),
                    ProposedID.Proposed(1),
                    ProposedID.Existing(992),
                    ProposedID.Existing(1636)
                ),
            ],
            ModifyElement1dProposals =
            [
                new ModifyElement1dProposal()
                {
                    ExistingElement1dId = 99,
                    EndNodeId = ProposedID.Proposed(1),
                },
            ],
        };

        var modelProposalResponseResult =
            await AssemblySetup.StructuralAnalysisApiClient.CreateModelProposalAsync(
                modelId,
                modelProposalRequest
            );

        await Verify(modelProposalResponseResult)
            .ScrubMembers(l =>
                typeof(IHasIntId).IsAssignableFrom(l.DeclaringType) && l.Name == "Id"
            );
    }

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
        var newNode1Response = await AssemblySetup.StructuralAnalysisApiClient.CreateNodeAsync(
            modelId,
            new CreateNodeRequest(
                new(7, 7, 7, LengthUnitContract.Foot),
                Restraint.Fixed,
                newNode1Id
            )
        );
        var newNode2Response = await AssemblySetup.StructuralAnalysisApiClient.CreateNodeAsync(
            modelId,
            new CreateNodeRequest(
                new(17, 17, 17, LengthUnitContract.Foot),
                Restraint.Fixed,
                newNode2Id
            )
        );
        newNode1Response.ThrowIfError();
        newNode2Response.ThrowIfError();

        var newElement1dResponse =
            await AssemblySetup.StructuralAnalysisApiClient.CreateElement1dAsync(
                modelId,
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

        var deleteNodeResponse = await AssemblySetup.StructuralAnalysisApiClient.DeleteNodeAsync(
            modelId,
            newNode1Id
        );
        deleteNodeResponse.ThrowIfError();

        // Verify that the element1d with id 1234 has been deleted
        var getElement1dResponse =
            await AssemblySetup.StructuralAnalysisApiClient.GetElement1dAsync(modelId, 1234);

        await Verify(getElement1dResponse)
            .ScrubInlineGuids()
            .ScrubMembers(l =>
                typeof(IHasIntId).IsAssignableFrom(l.DeclaringType) && l.Name == "Id"
            );
    }
}
