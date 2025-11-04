using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Sdk;
using FluentAssertions;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

[MethodDataSource(typeof(ApiClients), nameof(ApiClients.GetClients))]
public class OctreeIntegrationTests(ApiClientKey apiClientKey)
{
    private BeamOsResultApiClient ApiClient => field ??= apiClientKey.GetClient();

    [Test]
    public async Task CreateModelAndAddNode_ShouldAssignOctreeNodeId()
    {
        // Arrange
        Guid modelId = Guid.NewGuid();
        CreateModelRequest createModelRequest = new()
        {
            Name = "Octree Test Model",
            Description = "Test model for octree node assignment",
            Settings = new(UnitSettingsContract.K_FT),
            Id = modelId,
        };

        // Act - Create the model
        var modelResponse = await this.ApiClient.Models.CreateModelAsync(createModelRequest);
        modelResponse.IsSuccess.Should().BeTrue();

        // Act - Add a node to the model
        var modelClient = this.ApiClient.Models[modelId];
        CreateNodeRequest createNodeRequest = new(
            new(1, 2, 3, LengthUnitContract.Foot),
            Restraint.Free
        );

        var nodeResponse = await modelClient.Nodes.CreateNodeAsync(createNodeRequest);
        nodeResponse.IsSuccess.Should().BeTrue();

        // Assert - Node should have a valid OctreeNodeId assigned
        var node = nodeResponse.Value;
        node.Should().NotBeNull();
        node.OctreeNodeId.Should().BeGreaterThan(0);
    }
}
