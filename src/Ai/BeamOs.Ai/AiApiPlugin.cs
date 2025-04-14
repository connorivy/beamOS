using System.ComponentModel;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using Microsoft.SemanticKernel;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;

namespace BeamOs.Ai;

public class AiApiPlugin(IStructuralAnalysisApiClientV1 apiClient)
{
    [KernelFunction]
    [Description("Create a node in the model with the provided modelId.")]
    public Task<
        Result<NodeResponse>
    > CreateNodeAsync(
        [Description("Id of the model that the new node with be created in")] Guid modelId,
        [Description("X Coordinate of Node")] double locationPoint_x,
        [Description("Y Coordinate of Node")] double locationPoint_y,
        [Description("Z Coordinate of Node")] double locationPoint_z,
        string locationPoint_lengthUnit,
        bool restraint_canTranslateAlongX,
        bool restraint_canTranslateAlongY,
        bool restraint_canTranslateAlongZ,
        bool restraint_canRotateAboutX,
        bool restraint_canRotateAboutY,
        bool restraint_canRotateAboutZ,
        CancellationToken cancellationToken = default
    ) =>
        apiClient.CreateNodeAsync(
            modelId,
            new CreateNodeRequest(
                new Point(
                    locationPoint_x,
                    locationPoint_y,
                    locationPoint_z,

                    locationPoint_lengthUnit.MapToLengthUnitContract()
                ),
                new Restraint(
                    restraint_canTranslateAlongX,
                    restraint_canTranslateAlongY,
                    restraint_canTranslateAlongZ,
                    restraint_canRotateAboutX,
                    restraint_canRotateAboutY,
                    restraint_canRotateAboutZ
                )
            ),
            cancellationToken
        );
}
