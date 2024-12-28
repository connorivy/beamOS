using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;

namespace BeamOs.Tests.StructuralAnalysis.Integration.Api;

public class NodeTests
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
