using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Queries;
using BeamOs.Contracts.AnalyticalResults.AnalyticalNode;
using BeamOs.Contracts.Common;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Nodes.Endpoints;

public class GetNodeResults(
    BeamOsFastEndpointOptions options,
    IQueryHandler<GetModelResourcesByIdsQuery, NodeResultResponse?[]> getResourcesByIdsQueryHandler
) : BeamOsFastEndpoint<GetNodeResultsRequest, NodeResultResponse?[]>(options)
{
    public override Http EndpointType => Http.GET;
    public override string Route => "node-results/{modelId}";

    public override async Task<NodeResultResponse?[]> ExecuteAsync(
        GetNodeResultsRequest req,
        CancellationToken ct
    )
    {
        GetModelResourcesByIdsQuery query =
            new(Guid.Parse(req.ModelId), req.NodeIds?.Select(Guid.Parse).ToArray());

        return await getResourcesByIdsQueryHandler.ExecuteAsync(query, ct);
    }
}
