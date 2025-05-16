using System.ComponentModel;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using Microsoft.SemanticKernel;

namespace BeamOs.Ai;

public class AiApiPlugin(
    CreateNodeCommandHandler createNodeCommandHandler,
    DeleteNodeCommandHandler deleteNodeCommandHandler,
    GetNodeCommandHandler getNodeCommandHandler
)
{
    [KernelFunction]
    [Description("Create a node in the model with the provided modelId.")]
    public Task<Result<NodeResponse>> CreateNodeAsync(
        [Description("Id of the model that the new node with be created in")] Guid modelId,
        [Description("X Coordinate of Node")] double locationPoint_x,
        [Description("Y Coordinate of Node")] double locationPoint_y,
        [Description("Z Coordinate of Node")] double locationPoint_z,
        [Description("Length unit of the node coordinates")] LengthUnit lengthUnit,
        bool restraint_canTranslateAlongX,
        bool restraint_canTranslateAlongY,
        bool restraint_canTranslateAlongZ,
        bool restraint_canRotateAboutX,
        bool restraint_canRotateAboutY,
        bool restraint_canRotateAboutZ,
        CancellationToken cancellationToken = default
    ) =>
        createNodeCommandHandler.ExecuteAsync(
            new()
            {
                ModelId = modelId,
                Body = new CreateNodeRequest(
                    new Point(locationPoint_x, locationPoint_y, locationPoint_z, lengthUnit),
                    new Restraint(
                        restraint_canTranslateAlongX,
                        restraint_canTranslateAlongY,
                        restraint_canTranslateAlongZ,
                        restraint_canRotateAboutX,
                        restraint_canRotateAboutY,
                        restraint_canRotateAboutZ
                    )
                ),
            },
            cancellationToken
        );

    [KernelFunction]
    [Description("Delete a node in the model with the provided modelId.")]
    public Task<Result<ModelEntityResponse>> DeleteNodeAsync(
        [Description("Id of the model that the new node with be created in")] Guid modelId,
        [Description("Id of the node to be deleted")] int nodeId
    ) =>
        deleteNodeCommandHandler.ExecuteAsync(
            new ModelEntityCommand() { ModelId = modelId, Id = nodeId }
        );

    [KernelFunction]
    [Description("Delete a node in the model with the provided modelId.")]
    public Task<Result<NodeResponse>> GetNodeAsync(
        [Description("Id of the model that the new node with be created in")] Guid modelId,
        [Description("Id of the node to be deleted")] int nodeId
    ) =>
        getNodeCommandHandler.ExecuteAsync(
            new ModelEntityCommand() { ModelId = modelId, Id = nodeId }
        );
}
