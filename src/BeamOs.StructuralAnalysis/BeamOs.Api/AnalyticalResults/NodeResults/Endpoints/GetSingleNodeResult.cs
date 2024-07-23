using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.AnalyticalResults.AnalyticalNode;
using BeamOs.Contracts.Common;
using FastEndpoints;

namespace BeamOs.Api.AnalyticalResults.NodeResults.Endpoints;

public class GetSingleNodeResult(
    BeamOsFastEndpointOptions options,
    IQueryHandler<IdRequest, NodeResultResponse[]> getNodeResultsQueryHandler
) : BeamOsFastEndpoint<IdRequest, NodeResultResponse?[]>(options)
{
    public override Http EndpointType => Http.GET;
    public override string Route => "nodes/{id}/results";

    public override async Task<NodeResultResponse?[]> ExecuteAsync(
        IdRequest req,
        CancellationToken ct
    )
    {
        return await getNodeResultsQueryHandler.ExecuteAsync(req, ct);
    }
}
