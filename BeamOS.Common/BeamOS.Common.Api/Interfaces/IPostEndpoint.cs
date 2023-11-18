using System.Reflection;

namespace BeamOS.Common.Api.Interfaces;
public interface IPostEndpointBase
{
    Delegate PostAsyncDelegate { get; }
}
public interface IPostEndpoint<TRequest, TResponse> : IPostEndpointBase
    where TRequest : notnull
{
    Delegate IPostEndpointBase.PostAsyncDelegate
    {
        get
        {
            MethodInfo x = this.GetType().GetMethod(nameof(this.PostAsync));
            return DelegateHelpers.CreateDelegate(x, this);
        }
    }
    Task<TResponse> PostAsync(TRequest request, CancellationToken ct);
}

public interface IPostEndpoint<TRequest, TResponse, TParam1> : IPostEndpointBase
    where TRequest : notnull
{
    Delegate IPostEndpointBase.PostAsyncDelegate
    {
        get
        {
            MethodInfo x = this.GetType().GetMethod(nameof(this.PostAsync));
            return DelegateHelpers.CreateDelegate(x, this);
        }
    }
    public Task<TResponse> PostAsync(TRequest req, TParam1 param1, CancellationToken ct);
}

public interface IPostEndpoint<TRequest, TResponse, TParam1, TParam2> : IPostEndpointBase
    where TRequest : notnull
{
    Delegate IPostEndpointBase.PostAsyncDelegate
    {
        get
        {
            MethodInfo x = this.GetType().GetMethod(nameof(this.PostAsync));
            return DelegateHelpers.CreateDelegate(x, this);
        }
    }
    public Task<TResponse> PostAsync(TRequest request, TParam1 param1, TParam2 param2, CancellationToken ct);
}
