using BeamOS.Common.Api;
using BeamOS.Common.Api.Interfaces;
using BeamOS.DirectStiffnessMethod.Api.AnalyticalModels.Mappers;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalModels.Commands;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate;
using BeamOS.PhysicalModel.Client;
using BeamOS.PhysicalModel.Contracts.Model;

namespace BeamOS.DirectStiffnessMethod.Api.AnalyticalModels.Endpoints;

public class RunDirectStiffnessMethodEndpoint(
    IPhysicalModelAlphaClient physicalModelApi,
    ModelResponseToCreateAnalyticalModelCommand modelResponseMapper,
    CreateAnalyticalModelCommandHandler createAnalyticalModelCommandHandler
) : BaseEndpoint, IPostEndpoint<string, string>
{
    public override string Route => "/analytical-models/{id}";

    public async Task<string> PostAsync(string id, CancellationToken ct)
    {
        ModelResponse? modelResponse = await physicalModelApi.GetModelAsync(id, ct);

        CreateAnalyticalModelFromPhysicalModelCommand command = modelResponseMapper.Map(
            modelResponse
        );

        AnalyticalModel? model = await createAnalyticalModelCommandHandler.ExecuteAsync(
            command,
            ct
        );

        return "success";
    }
}
