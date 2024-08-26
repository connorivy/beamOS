using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Application.Common.Queries;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Common.Identity.Policies;
using BeamOs.Contracts.AnalyticalResults.AnalyticalNode;
using BeamOs.Contracts.Common;
using FastEndpoints;

namespace BeamOs.Api.AnalyticalResults.NodeResults.Endpoints;

public class GetNodeResults(
    BeamOsFastEndpointOptions options,
    IQueryHandler<GetModelResourcesByIdsQuery, NodeResultResponse?[]> getResourcesByIdsQueryHandler
) : BeamOsFastEndpoint<GetNodeResultsRequest, ICollection<NodeResultResponse>>(options)
{
    public override Http EndpointType => Http.GET;
    public override string Route => "models/{modelId}/node-results";

    public override void ConfigureAuthentication()
    {
        this.AllowAnonymous();
        this.Policy(p => p.Requirements.Add(new RequireModelReadAccess()));
    }

    public override async Task<ICollection<NodeResultResponse>> ExecuteRequestAsync(
        GetNodeResultsRequest req,
        CancellationToken ct
    )
    {
        GetModelResourcesByIdsQuery query =
            new(Guid.Parse(req.ModelId), new(req.NodeIds?.Select(Guid.Parse) ?? []));

        return await getResourcesByIdsQueryHandler.ExecuteAsync(query, ct);
    }
}
