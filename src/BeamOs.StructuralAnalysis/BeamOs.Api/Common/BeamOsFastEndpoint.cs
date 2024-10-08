using System.Security.Claims;
using BeamOs.Api;
using BeamOs.Api.Common;
using BeamOs.Common.Api;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication;

namespace BeamOS.Api.Common;

public abstract class BeamOsFastEndpoint<TRequest, TResponse>(
    BeamOsFastEndpointOptions? options = null
) : Endpoint<TRequest, TResponse>, IBeamOsEndpoint<TRequest, TResponse>
    where TRequest : notnull
{
    public abstract Task<TResponse> ExecuteRequestAsync(TRequest req, CancellationToken ct);
    public abstract Http EndpointType { get; }
    public abstract string Route { get; }

    public StructuralAnalysisMetrics StructuralAnalysisMetrics { get; set; }

    public virtual void ConfigureAuthentication()
    {
        if (options?.DefaultAuthenticationConfiguration is not null)
        {
            options.DefaultAuthenticationConfiguration(this);
        }
        else
        {
            this.AllowAnonymous();
        }
    }

    protected virtual void ConfigureEndpoint() { }

    public virtual TRequest? ExampleRequest { get; }

    public sealed override void Configure()
    {
        this.Verbs(this.EndpointType);
        this.Routes(this.Route);
        this.ConfigureAuthentication();
        this.ConfigureEndpoint();

        if (this.ExampleRequest is not null)
        {
            this.Summary(s => s.ExampleRequest = this.ExampleRequest);
        }
    }

    public override async Task HandleAsync(TRequest req, CancellationToken ct)
    {
        Result<TResponse> requestResult;
        try
        {
            requestResult = await this.ExecuteRequestAsync(req, ct);
            await requestResult.Match(this.DispatchSucessResponse, this.DispatchErrorResponse);
        }
        finally
        {
            this.StructuralAnalysisMetrics.RecordMetrics(this.GetType().Name);
        }
    }

    private async Task DispatchSucessResponse(TResponse response)
    {
        if (this.HttpContext.Response.StatusCode == 200)
        {
            await this.SendOkAsync(response);
        }
    }

    private async Task DispatchErrorResponse(Exception exception)
    {
        if (exception is UnauthorizedException)
        {
            await this.SendUnauthorizedAsync();
        }
        else
        {
            // todo
        }
    }

    public object ExecuteRequestAsync(object request, CancellationToken ct) =>
        this.ExecuteRequestAsync((TRequest)request, ct);
}

public interface IRequestValidator<TRequest>
    where TRequest : notnull
{
    public Exception? ValidateRequest(TRequest req);
}

//public class ModelRequestValidator : IRequestValidator<IdRequestWithProperties>
//{
//    private readonly IHttpContextAccessor contextAccessor;

//    public ModelRequestValidator(IHttpContextAccessor contextAccessor)
//    {
//        this.contextAccessor = contextAccessor;
//    }

//    public Exception? ValidateRequest(IdRequestWithProperties req)
//    {
//        if ()
//    }
//}

public sealed class UserPermissionHydrator : IClaimsTransformation
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public UserPermissionHydrator(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        ArgumentNullException.ThrowIfNull(email);

        if (principal.Claims.FirstOrDefault(c => c.Type == "permissions") is not null)
        {
            return Task.FromResult(principal);
        }

        if (email == "user@email.com")
        {
            principal.AddIdentity(new([new Claim("permissions", "Do_A_Thing")]));
        }

        return Task.FromResult(principal);
    }
}

public class UnauthorizedException : Exception { }
