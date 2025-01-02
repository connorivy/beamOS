using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;

namespace BeamOs.Tests.StructuralAnalysis.Integration.Api;

public class EndToEndTests
{
    private static Guid modelId;
    private static Result<ModelResponse> modelResponseResult;

    [Before(Class)]
    public static async Task SetupModel()
    {
        modelId = Guid.NewGuid();

        CreateModelRequest request =
            new()
            {
                Name = "test model",
                Description = "test model",
                Settings = new(UnitSettingsContract.K_FT),
                Id = modelId
            };

        modelResponseResult = await AssemblySetup
            .StructuralAnalysisApiClient
            .CreateModelAsync(request);
    }

    [Test]
    public async Task CreateModel_ShouldReturnSuccessfulResponse()
    {
        await Verify(modelResponseResult);
    }

    [Test]
    public async Task CreateNode_WithNoSpecifiedId_ShouldCreateNode_AndGiveAnId()
    {
        CreateNodeRequest createNodeRequestBody =
            new(new(1, 1, 1, LengthUnitContract.Foot), Restraint.Fixed);

        var nodeResponseResult = await AssemblySetup
            .StructuralAnalysisApiClient
            .CreateNodeAsync(modelId, createNodeRequestBody);

        await Verify(nodeResponseResult);
    }

    [Test]
    public async Task CreateNode_WithIdOf5_ShouldCreateNode_WithCorrectId()
    {
        CreateNodeRequest createNodeRequestBody =
            new(new(1, 1, 1, LengthUnitContract.Foot), Restraint.Fixed, 5);

        var nodeResponseResult = await AssemblySetup
            .StructuralAnalysisApiClient
            .CreateNodeAsync(modelId, createNodeRequestBody);

        await Verify(nodeResponseResult);
    }

    [Test]
    [DependsOn(nameof(CreateNode_WithIdOf5_ShouldCreateNode_WithCorrectId))]
    public async Task CreatePointLoad_ShouldCreatePointLoad()
    {
        CreatePointLoadRequest requestBody =
            new()
            {
                NodeId = 5,
                Direction = new(0, -1, 0),
                Force = new(10, ForceUnitContract.KilopoundForce),
                Id = 5
            };

        var result = await AssemblySetup
            .StructuralAnalysisApiClient
            .CreatePointLoadAsync(modelId, requestBody);

        await Verify(result);
    }

    [Test]
    public async Task CreateSectionProfile_WithSpecifiedId_ShouldCreateSectionProfile_WithCorrectId()
    {
        CreateSectionProfileRequest w16x36Request =
            new()
            {
                Area = new AreaContract(10.6, AreaUnitContract.SquareInch),
                StrongAxisMomentOfInertia = new AreaMomentOfInertiaContract(
                    448,
                    AreaMomentOfInertiaUnitContract.InchToTheFourth
                ),
                WeakAxisMomentOfInertia = new AreaMomentOfInertiaContract(
                    24.5,
                    AreaMomentOfInertiaUnitContract.InchToTheFourth
                ),
                PolarMomentOfInertia = new AreaMomentOfInertiaContract(
                    .55,
                    AreaMomentOfInertiaUnitContract.InchToTheFourth
                ),
                StrongAxisShearArea = new AreaContract(5.0095, AreaUnitContract.SquareInch),
                WeakAxisShearArea = new AreaContract(4.6905, AreaUnitContract.SquareInch),
                Id = 1636
            };

        var sectionProfileResponseResult = await AssemblySetup
            .StructuralAnalysisApiClient
            .CreateSectionProfileAsync(modelId, w16x36Request);

        await Verify(sectionProfileResponseResult);
    }

    [Test]
    public async Task CreateMaterial_WithSpecifiedId_ShouldCreateMaterial_WithCorrectId()
    {
        CreateMaterialRequest a992Request =
            new()
            {
                ModulusOfElasticity = new(29000, PressureUnitContract.KilopoundForcePerSquareInch),
                ModulusOfRigidity = new(
                    11_153.85,
                    PressureUnitContract.KilopoundForcePerSquareInch
                ),
                Id = 992
            };

        var materialResponseResult = await AssemblySetup
            .StructuralAnalysisApiClient
            .CreateMaterialAsync(modelId, a992Request);

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
        CreateNodeRequest createNodeRequestBody =
            new(new(1, 1, 1, LengthUnitContract.Foot), Restraint.Fixed, 6);

        var nodeResponseResult = await AssemblySetup
            .StructuralAnalysisApiClient
            .CreateNodeAsync(modelId, createNodeRequestBody);

        await Assert.That(nodeResponseResult.IsSuccess).IsTrue();

        CreateElement1dRequest elRequest =
            new()
            {
                StartNodeId = 5,
                EndNodeId = 6,
                MaterialId = 992,
                SectionProfileId = 1636,
                Id = 99,
            };

        var elResponseResult = await AssemblySetup
            .StructuralAnalysisApiClient
            .CreateElement1dAsync(modelId, elRequest);

        await Verify(elResponseResult);
    }

    [Test]
    [DependsOn(nameof(CreateElement1d_ShouldCreateElement1d))]
    public async Task GetElement1d_ShouldResultInExpectedResponse()
    {
        var elResponseResult = await AssemblySetup
            .StructuralAnalysisApiClient
            .GetElement1dAsync(modelId, 99);

        await Verify(elResponseResult);
    }

    [Test]
    [DependsOn(nameof(CreateNode_WithIdOf5_ShouldCreateNode_WithCorrectId))]
    public async Task UpdateNode_WithPartialLocation_ShouldPartiallyUpdate()
    {
        UpdateNodeRequest updateNodeRequest =
            new(5, new() { LengthUnit = LengthUnitContract.Meter, X = 50 }, Restraint.Free);

        var nodeResponseResult = await AssemblySetup
            .StructuralAnalysisApiClient
            .UpdateNodeAsync(modelId, updateNodeRequest);

        await Verify(nodeResponseResult);
    }
}
