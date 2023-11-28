using BeamOS.Common.Api;
using BeamOS.Common.Api.Interfaces;
using BeamOS.Common.Application.Interfaces;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalModels.Commands;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate;
using BeamOS.PhysicalModel.Contracts.Model;

namespace BeamOS.DirectStiffnessMethod.Api.AnalyticalModels.Endpoints;

public class RunDirectStiffnessMethodEndpoint(
    PhysicalModelApiClient physicalModelApi,
    IMapper<ModelResponse, CreateAnalyticalModelCommand> modelResponseMapper,
    ICommandHandler<CreateAnalyticalModelCommand, AnalyticalModel> createAnalyticalModelCommandHandler)
    : BaseEndpoint, IPostEndpoint<string, string>
{
    public override string Route => "/analytical-models/{id}";

    public async Task<string> PostAsync(string id, CancellationToken ct)
    {
        ModelResponse? modelResponse = await physicalModelApi.GetModelResponse(id);

        CreateAnalyticalModelCommand command = modelResponseMapper.Map(modelResponse);

        AnalyticalModel model = await createAnalyticalModelCommandHandler.ExecuteAsync(command, ct);

        return null;
    }
}
