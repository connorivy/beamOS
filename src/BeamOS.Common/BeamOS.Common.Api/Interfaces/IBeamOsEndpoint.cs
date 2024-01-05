using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace BeamOS.Common.Api.Interfaces;

public interface IBeamOsEndpointBase
{
    public Delegate GetExecuteAsyncDelegate();

    public abstract string Route { get; }

    public abstract EndpointType EndpointType { get; }

    //private Delegate GetExecuteAsyncDelegate()
    //{
    //    MethodInfo x =
    //        this.GetType().GetMethod("ExecuteAsync")
    //        ?? throw new Exception(
    //            $"BaseEndpoint must implement an overload of the IBeamOsEndpoint interface"
    //        );
    //    return DelegateHelpers.CreateDelegate(x, this);
    //}

    private delegate RouteHandlerBuilder EndpointMap(string pattern, Delegate handler);

    public void Map(IEndpointRouteBuilder app)
    {
        EndpointMap endpointMap = this.EndpointType switch
        {
            EndpointType.Delete => app.MapDelete,
            EndpointType.Get => app.MapGet,
            EndpointType.Patch => app.MapPatch,
            EndpointType.Post => app.MapPost,
            EndpointType.Put => app.MapPut,

            EndpointType.Undefined or _ => throw new NotImplementedException(),
        };

        _ = endpointMap(this.Route, this.GetExecuteAsyncDelegate()).WithName(this.GetName());
    }

    private string GetName() => this.GetType().Name;
}

public interface IBeamOsEndpoint<TRequest, TResponse> : IBeamOsEndpointBase
    where TRequest : notnull
{
    Delegate IBeamOsEndpointBase.GetExecuteAsyncDelegate()
    {
        MethodInfo x = this.GetType().GetMethod(nameof(this.ExecuteAsync));
        return DelegateHelpers.CreateDelegate(x, this);
    }
    static abstract Task<TResponse> ExecuteAsync(TRequest request, CancellationToken ct);
}

public interface IBeamOsEndpoint<TRequest, TResponse, TParam1> : IBeamOsEndpointBase
    where TRequest : notnull
{
    Delegate IBeamOsEndpointBase.GetExecuteAsyncDelegate()
    {
        MethodInfo x = this.GetType().GetMethod(nameof(this.ExecuteAsync));
        return DelegateHelpers.CreateDelegate(x, this);
    }
    public static abstract Task<TResponse> ExecuteAsync(
        TRequest req,
        TParam1 param1,
        CancellationToken ct
    );
}

public interface IBeamOsEndpoint<TRequest, TResponse, TParam1, TParam2> : IBeamOsEndpointBase
    where TRequest : notnull
{
    Delegate IBeamOsEndpointBase.GetExecuteAsyncDelegate()
    {
        MethodInfo x = this.GetType().GetMethod(nameof(this.ExecuteAsync));
        return DelegateHelpers.CreateDelegate(x, this);
    }
    public Task<TResponse> ExecuteAsync(
        TRequest request,
        TParam1 param1,
        TParam2 param2,
        CancellationToken ct
    );
}

public static class DelegateHelpers
{
    public static Delegate CreateDelegate(this MethodInfo methodInfo, object target)
    {
        Func<Type[], Type> getType;
        var isAction = methodInfo.ReturnType.Equals(typeof(void));
        var types = methodInfo.GetParameters().Select(p => p.ParameterType);

        if (isAction)
        {
            getType = Expression.GetActionType;
        }
        else
        {
            getType = Expression.GetFuncType;
            types = types.Concat(new[] { methodInfo.ReturnType });
        }

        if (methodInfo.IsStatic)
        {
            return Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);
        }

        return Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo.Name);
    }
}
