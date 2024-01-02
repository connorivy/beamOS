using BeamOS.Common.Api;
using BeamOS.Common.Api.Interfaces;
using BeamOS.DirectStiffnessMethod.Api.AnalyticalModels.Mappers;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalModels.Commands;
using BeamOS.DirectStiffnessMethod.Contracts.Model;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate;
using BeamOS.PhysicalModel.Client;
using BeamOS.PhysicalModel.Contracts.Model;

namespace BeamOS.DirectStiffnessMethod.Api.AnalyticalModels.Endpoints;

public class RunDirectStiffnessMethod(
    IPhysicalModelAlphaClient physicalModelApi,
    ModelResponseHydratedToCreateAnalyticalModelCommand modelResponseMapper,
    CreateAnalyticalModelCommandHandler createAnalyticalModelCommandHandler
) : BaseEndpoint, IPostEndpoint<string, AnalyticalModelResponse>
{
    public override string Route => "/analytical-models/{physicalModelId}";

    public async Task<AnalyticalModelResponse> PostAsync(
        string physicalModelId,
        CancellationToken ct
    )
    {
        ModelResponseHydrated modelResponse = await physicalModelApi.GetModelHydratedAsync(
            physicalModelId,
            ct
        );

        CreateAnalyticalModelFromPhysicalModelCommand command = modelResponseMapper.Map(
            modelResponse
        );

        AnalyticalModel model =
            await createAnalyticalModelCommandHandler.ExecuteAsync(command, ct)
            ?? throw new Exception("Analytical model returned null");

        List<UnsupportedStructureDisplacementIdResponse> degreeOfFreedomIdResponses = model
            .DegreeOfFreedomIds
            .Select(
                id =>
                    new UnsupportedStructureDisplacementIdResponse(
                        id.NodeId.ToString(),
                        id.Direction.ToString()
                    )
            )
            .ToList();

        List<UnsupportedStructureDisplacementIdResponse> boundaryConditionIdResponses = model
            .BoundaryConditionIds
            .Select(
                id =>
                    new UnsupportedStructureDisplacementIdResponse(
                        id.NodeId.ToString(),
                        id.Direction.ToString()
                    )
            )
            .ToList();

        return new AnalyticalModelResponse(
            degreeOfFreedomIdResponses,
            boundaryConditionIdResponses,
            model.JointDisplacementVector.Select(kvp => kvp.Value).ToList(),
            model.ReactionVector.Select(kvp => kvp.Value).ToList()
        );
    }
}
