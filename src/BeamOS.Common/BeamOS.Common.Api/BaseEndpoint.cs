using BeamOS.Common.Api.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace BeamOS.Common.Api;

public abstract class BaseEndpoint
{
    public abstract string Route { get; }

    public void Map(IEndpointRouteBuilder app)
    {
        if (this is IGetEndpointBase getEndpoint)
        {
            _ = app.MapGet(this.Route, getEndpoint.GetAsyncDelegate).WithName(this.GetName());
        }
        if (this is IPostEndpointBase postEndpoint)
        {
            _ = app.MapPost(this.Route, postEndpoint.PostAsyncDelegate).WithName(this.GetName());
        }
    }

    private string GetName() => this.GetType().Name;
}
