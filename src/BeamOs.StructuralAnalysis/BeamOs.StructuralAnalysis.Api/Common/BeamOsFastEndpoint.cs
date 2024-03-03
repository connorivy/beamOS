using BeamOs.Api.Common.Interfaces;
using FastEndpoints;

namespace BeamOS.Api.Common;

public abstract class BeamOsFastEndpoint<TRequest, TResponse>(BeamOsFastEndpointOptions options)
    : Endpoint<TRequest, TResponse>,
        IBeamOsEndpoint<TRequest, TResponse>
    where TRequest : notnull
{
    public abstract override Task<TResponse> ExecuteAsync(TRequest req, CancellationToken ct);
    public abstract Http EndpointType { get; }
    public abstract string Route { get; }

    public virtual void ConfigureAuthentication()
    {
        if (options.DefaultAuthenticationConfiguration is not null)
        {
            options.DefaultAuthenticationConfiguration(this);
        }
        else
        {
            this.AllowAnonymous();
        }
    }

    public override void Configure()
    {
        this.Verbs(this.EndpointType);
        this.Routes(this.Route);
        this.ConfigureAuthentication();
    }
}
