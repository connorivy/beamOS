using BeamOs.Api.AnalyticalResults.AnalyticalNodes.Mappers;
using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Api.DirectStiffnessMethod.AnalyticalModels.Mappers;
using BeamOs.Application.DirectStiffnessMethod.AnalyticalModels.Commands;
using BeamOs.Contracts.AnalyticalResults.Model;
using BeamOs.Contracts.PhysicalModel.Model;
using FastEndpoints;

namespace BeamOs.Api.DirectStiffnessMethod.AnalyticalModels.Endpoints;

public class RunDirectStiffnessMethod(
    BeamOsFastEndpointOptions options,
    ModelResponseHydratedToCreateAnalyticalModelCommand modelResponseMapper,
    CreateAnalyticalModelCommandHandler createAnalyticalModelCommandHandler,
    AnalyticalNodeResponseMapper analyticalNodeResponseMapper
) : BeamOsFastEndpoint<ModelResponseHydrated, AnalyticalModelResponse>(options)
{
    public override string Route => "/direct-stiffness-method/run";

    public override Http EndpointType => Http.POST;

    public override async Task<AnalyticalModelResponse> ExecuteAsync(
        ModelResponseHydrated modelResponse,
        CancellationToken ct
    )
    {
        var command = modelResponseMapper.Map(modelResponse);

        var model =
            await createAnalyticalModelCommandHandler.ExecuteAsync(command, ct)
            ?? throw new Exception("Analytical model returned null");

        var degreeOfFreedomIdResponses = model
            .DegreeOfFreedomIds
            .Select(
                id =>
                    new UnsupportedStructureDisplacementIdResponse(
                        id.NodeId.ToString(),
                        id.Direction.ToString()
                    )
            )
            .ToList();

        var boundaryConditionIdResponses = model
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
            model.UnknownJointDisplacementVector.Select(kvp => kvp.Value).ToList(),
            model.UnknownReactionVector.Select(kvp => kvp.Value).ToList()
        );
    }
}
