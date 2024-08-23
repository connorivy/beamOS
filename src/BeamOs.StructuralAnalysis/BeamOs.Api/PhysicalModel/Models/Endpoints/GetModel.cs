using System.Security.Claims;
using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Application.Common.Queries;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Common.Identity.Policies;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Models.Endpoints;

public class GetModel(
    BeamOsFastEndpointOptions options,
    IQueryHandler<GetResourceByIdWithPropertiesQuery, ModelResponse> getResourceByIdQueryHandler
) : BeamOsFastEndpoint<ModelIdRequestWithProperties, ModelResponse?>(options)
{
    public override string Route => "models/{modelId}";

    public override Http EndpointType => Http.GET;

    public override void ConfigureAuthentication()
    {
        this.AllowAnonymous();
        this.Policy(p => p.AddRequirements(new RequireModelReadAccess()));
    }

    public override async Task<ModelResponse?> ExecuteRequestAsync(
        ModelIdRequestWithProperties req,
        CancellationToken ct
    )
    {
        GetResourceByIdWithPropertiesQuery query =
            new(Guid.Parse(req.ModelId), req.Properties?.Length > 0 ? req.Properties : null);

        return await getResourceByIdQueryHandler.ExecuteAsync(query, ct);
    }
}

public class GetModels(
    BeamOsFastEndpointOptions options,
    IQueryHandler<ClaimsPrincipal, List<ModelResponse>> getModelResponsesQueryHandler
) : BeamOsFastEndpoint<FastEndpoints.EmptyRequest, List<ModelResponse>>(options)
{
    public override string Route => "models";

    public override Http EndpointType => Http.GET;

    //public override void ConfigureAuthentication()
    //{
    //    this.AllowAnonymous();
    //}

    public override async Task<List<ModelResponse>> ExecuteRequestAsync(
        FastEndpoints.EmptyRequest req,
        CancellationToken ct
    )
    {
        return await getModelResponsesQueryHandler.ExecuteAsync(HttpContext.User, ct);
    }
}
