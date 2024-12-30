using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
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
    public async Task CreateNode_WithSpecifiedId_ShouldCreateNode_WithCorrectId()
    {
        CreateNodeRequest createNodeRequestBody =
            new(new(1, 1, 1, LengthUnitContract.Foot), Restraint.Fixed, 5);

        var nodeResponseResult = await AssemblySetup
            .StructuralAnalysisApiClient
            .CreateNodeAsync(modelId, createNodeRequestBody);

        await Verify(nodeResponseResult);
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
    [DependsOn(nameof(CreateNode_WithSpecifiedId_ShouldCreateNode_WithCorrectId))]
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
