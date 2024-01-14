using System.Linq.Expressions;
using System.Reflection;
using BeamOS.Common.Api.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace BeamOS.Common.Api;

public abstract class BeamOsEndpointBase : IBeamOsEndpointBase
{
    public abstract string Route { get; }

    public abstract EndpointType EndpointType { get; }

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

    private Delegate GetExecuteAsyncDelegate()
    {
        MethodInfo x =
            this.GetType().GetMethod("ExecuteAsync")
            ?? throw new Exception(
                $"BaseEndpoint must implement an overload of the IBeamOsEndpoint interface"
            );
        return CreateDelegate(x, this);
    }

    private static Delegate CreateDelegate(MethodInfo methodInfo, object target)
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

    private delegate RouteHandlerBuilder EndpointMap(string pattern, Delegate handler);

    private string GetName() => this.GetType().Name;
}

public abstract class BeamOsEndpoint<TRequest, TResponse>
    : BeamOsEndpointBase,
        IBeamOsEndpoint<TRequest, TResponse>
{
    public abstract Task<TResponse> ExecuteAsync(TRequest request, CancellationToken ct);
}

public abstract class BeamOsEndpoint<TRequest, TParam1, TResponse>
    : BeamOsEndpointBase,
        IBeamOsEndpoint<TRequest, TParam1, TResponse>
{
    public abstract Task<TResponse> ExecuteAsync(
        TRequest request,
        TParam1 param1,
        CancellationToken ct
    );
}

public abstract class BeamOsEndpoint<TRequest, TParam1, TParam2, TResponse>
    : BeamOsEndpointBase,
        IBeamOsEndpoint<TRequest, TParam1, TParam2, TResponse>
{
    public abstract Task<TResponse> ExecuteAsync(
        TRequest request,
        TParam1 param1,
        TParam2 param2,
        CancellationToken ct
    );
}

public abstract class BeamOsEndpoint<TRequest, TParam1, TParam2, TParam3, TResponse>
    : BeamOsEndpointBase,
        IBeamOsEndpoint<TRequest, TParam1, TParam2, TParam3, TResponse>
{
    public abstract Task<TResponse> ExecuteAsync(
        TRequest request,
        TParam1 param1,
        TParam2 param2,
        TParam3 param3,
        CancellationToken ct
    );
}

public enum EndpointType
{
    Undefined = 0,
    Delete,
    Get,
    Patch,
    Post,
    Put,
}
