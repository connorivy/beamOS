using System.Linq.Expressions;
using System.Reflection;

namespace BeamOS.Common.Api.Interfaces;
public interface IGetEndpointBase
{
    Delegate GetAsyncDelegate { get; }
}
public interface IGetEndpoint<TRequest, TResponse> : IGetEndpointBase
    where TRequest : notnull
{
    Delegate IGetEndpointBase.GetAsyncDelegate
    {
        get
        {
            MethodInfo x = this.GetType().GetMethod(nameof(this.GetAsync));
            return DelegateHelpers.CreateDelegate(x, this);
        }
    }
    Task<TResponse> GetAsync(TRequest request, CancellationToken ct);
}

public interface IGetEndpoint<TRequest, TResponse, TParam1> : IGetEndpointBase
    where TRequest : notnull
{
    Delegate IGetEndpointBase.GetAsyncDelegate
    {
        get
        {
            MethodInfo x = this.GetType().GetMethod(nameof(this.GetAsync));
            return DelegateHelpers.CreateDelegate(x, this);
        }
    }
    public Task<TResponse> GetAsync(TRequest req, TParam1 param1, CancellationToken ct);
}

public interface IGetEndpoint<TRequest, TResponse, TParam1, TParam2> : IGetEndpointBase
    where TRequest : notnull
{
    Delegate IGetEndpointBase.GetAsyncDelegate
    {
        get
        {
            MethodInfo x = this.GetType().GetMethod(nameof(this.GetAsync));
            return DelegateHelpers.CreateDelegate(x, this);
        }
    }
    public Task<TResponse> GetAsync(TRequest request, TParam1 param1, TParam2 param2, CancellationToken ct);
}

public static class DelegateHelpers
{
    public static Delegate CreateDelegate(this MethodInfo methodInfo, object target)
    {
        Func<Type[], Type> getType;
        var isAction = methodInfo.ReturnType.Equals((typeof(void)));
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
