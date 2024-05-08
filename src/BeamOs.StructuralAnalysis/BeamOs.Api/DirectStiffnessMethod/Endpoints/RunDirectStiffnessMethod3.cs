using BeamOs.Api.AnalyticalResults.NodeResults.Mappers;
using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Application.DirectStiffnessMethod.Commands;
using BeamOs.Contracts.AnalyticalResults.Model;
using BeamOs.Contracts.Common;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using FastEndpoints;

namespace BeamOs.Api.DirectStiffnessMethod.Endpoints;

public class RunDirectStiffnessMethod(
    BeamOsFastEndpointOptions options,
    RunDirectStiffnessMethodCommandHandler runDsmCommandHandler,
    NodeResultToResponseMapper nodeResultToResponseMapper
) : BeamOsFastEndpoint<IdRequest, AnalyticalModelResponse3>(options)
{
    public override string Route => "/direct-stiffness-method/run/";

    public override Http EndpointType => Http.POST;

    public override async Task<AnalyticalModelResponse3> ExecuteAsync(
        IdRequest req,
        CancellationToken ct
    )
    {
        var command = new RunDirectStiffnessMethodCommand(new ModelId(Guid.Parse(req.Id)));

        var model = await runDsmCommandHandler.ExecuteAsync(command, ct);

        var nodeResponses = nodeResultToResponseMapper.Map(model.NodeResults).ToList();

        return new AnalyticalModelResponse3(nodeResponses);
    }
}
