using BeamOs.Api.AnalyticalResults.NodeResults.Mappers;
using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Application.AnalyticalModel.ModelResults;
using BeamOs.Application.DirectStiffnessMethod.Commands;
using BeamOs.Contracts.AnalyticalResults.Model;
using BeamOs.Contracts.Common;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using FastEndpoints;

namespace BeamOs.Api.DirectStiffnessMethod.Endpoints;

public class RunDirectStiffnessMethod(
    BeamOsFastEndpointOptions options,
    RunDirectStiffnessMethodCommandHandler runDsmCommandHandler,
    NodeResultToResponseMapper nodeResultToResponseMapper,
    IModelResultRepository modelResultRepository
) : BeamOsFastEndpoint<ModelIdRequest, bool>(options)
{
    public override string Route => "models/{modelId}/analyze/direct-stiffness-method";

    public override Http EndpointType => Http.GET;

    public override async Task<bool> ExecuteRequestAsync(ModelIdRequest req, CancellationToken ct)
    {
        ModelId modelId = new(Guid.Parse(req.ModelId));
        //if (await modelResultRepository.GetByModelId(modelId) is not null)
        //{
        //    // todo: better response
        //    return new([]);
        //}
        var command = new RunDirectStiffnessMethodCommand(new ModelId(Guid.Parse(req.ModelId)));

        return await runDsmCommandHandler.ExecuteAsync(command, ct);

        //var nodeResponses = nodeResultToResponseMapper.Map(model.NodeResults).ToList();

        //return new AnalyticalModelResponse3(nodeResponses);
    }
}
