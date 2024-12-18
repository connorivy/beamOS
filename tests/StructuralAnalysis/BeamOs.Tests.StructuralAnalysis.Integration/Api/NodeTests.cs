using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;

namespace BeamOs.Tests.StructuralAnalysis.Integration.Api;

public class NodeTests
{
    private static readonly Guid ModelId = Guid.NewGuid();

    [Test]
    public async Task CreateNode_WithNoSpecifiedId_ShouldCreateNode_AndGiveAnId()
    {
        CreateNodeRequest createNodeRequestBody =
            new(new(1, 1, 1, LengthUnitContract.Foot), Restraint.Fixed);

        var nodeResponseResult = await AssemblySetup
            .StructuralAnalysisApiClient
            .CreateNodeAsync(ModelId, createNodeRequestBody);

        await Verify(nodeResponseResult);
    }

    [Test]
    public async Task CreateNode_WithSpecifiedId_ShouldCreateNode_WithCorrectId()
    {
        CreateNodeRequest createNodeRequestBody =
            new(new(1, 1, 1, LengthUnitContract.Foot), Restraint.Fixed, 5);

        var nodeResponseResult = await AssemblySetup
            .StructuralAnalysisApiClient
            .CreateNodeAsync(ModelId, createNodeRequestBody);

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
            .UpdateNodeAsync(ModelId, updateNodeRequest);

        await Verify(nodeResponseResult);
    }
}
