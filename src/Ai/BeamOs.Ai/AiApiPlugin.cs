using System.ComponentModel;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using Microsoft.SemanticKernel;

namespace BeamOs.Ai;

public class AiApiPlugin
{
    private readonly List<CreateNodeRequest> createNodeProposals = new();
    private readonly List<PutNodeRequest> modifyNodeProposals = new();

    [KernelFunction]
    [Description("Create a node in the model.")]
    public void CreateNodeAsync(
        [Description("Id that you can use to reference the node later")] int nodeId,
        [Description("X Coordinate of Node")] double locationPoint_x,
        [Description("Y Coordinate of Node")] double locationPoint_y,
        [Description("Z Coordinate of Node")] double locationPoint_z,
        [Description("Length unit of the node coordinates")] LengthUnit lengthUnit,
        bool restraint_canTranslateAlongX,
        bool restraint_canTranslateAlongY,
        bool restraint_canTranslateAlongZ,
        bool restraint_canRotateAboutX,
        bool restraint_canRotateAboutY,
        bool restraint_canRotateAboutZ
    ) =>
        this.createNodeProposals.Add(
            new CreateNodeRequest(
                new Point(locationPoint_x, locationPoint_y, locationPoint_z, lengthUnit),
                new Restraint(
                    restraint_canTranslateAlongX,
                    restraint_canTranslateAlongY,
                    restraint_canTranslateAlongZ,
                    restraint_canRotateAboutX,
                    restraint_canRotateAboutY,
                    restraint_canRotateAboutZ
                ),
                nodeId
            )
        );

    [KernelFunction]
    [Description("Modify a node in the model")]
    public void ModifyNodeAsync(
        [Description("Id of the node to be modified")] int nodeId,
        [Description("X Coordinate of Node")] double locationPoint_x,
        [Description("Y Coordinate of Node")] double locationPoint_y,
        [Description("Z Coordinate of Node")] double locationPoint_z,
        [Description("Length unit of the node coordinates")] LengthUnit lengthUnit,
        bool restraint_canTranslateAlongX,
        bool restraint_canTranslateAlongY,
        bool restraint_canTranslateAlongZ,
        bool restraint_canRotateAboutX,
        bool restraint_canRotateAboutY,
        bool restraint_canRotateAboutZ
    ) =>
        this.modifyNodeProposals.Add(
            new PutNodeRequest(
                nodeId,
                new Point(locationPoint_x, locationPoint_y, locationPoint_z, lengthUnit),
                new Restraint(
                    restraint_canTranslateAlongX,
                    restraint_canTranslateAlongY,
                    restraint_canTranslateAlongZ,
                    restraint_canRotateAboutX,
                    restraint_canRotateAboutY,
                    restraint_canRotateAboutZ
                )
            )
        );

    // [KernelFunction]
    // [Description("Delete a node in the model with the provided modelId.")]
    // public Task<Result<ModelEntityResponse>> DeleteNodeAsync(
    //     [Description("Id of the model that the new node with be created in")] Guid modelId,
    //     [Description("Id of the node to be deleted")] int nodeId
    // ) =>
    //     deleteNodeCommandHandler.ExecuteAsync(
    //         new ModelEntityCommand() { ModelId = modelId, Id = nodeId }
    //     );

    // [KernelFunction]
    // [Description("Gets information about the node with the provided id")]
    // public Task<Result<NodeResponse>> GetNodeAsync(
    //     [Description("Id of the model that the new node with be created in")] Guid modelId,
    //     [Description("Id of the node to be deleted")] int nodeId
    // ) =>
    //     getNodeCommandHandler.ExecuteAsync(
    //         new ModelEntityCommand() { ModelId = modelId, Id = nodeId }
    //     );

    public ModelProposalData GetModelProposalData()
    {
        return new()
        {
            CreateNodeProposals = this.createNodeProposals,
            ModifyNodeProposals = this.modifyNodeProposals,
        };
    }
}
