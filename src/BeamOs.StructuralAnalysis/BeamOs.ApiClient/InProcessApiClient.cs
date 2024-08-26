using System.Reflection;
using BeamOs.CodeGen.Apis.StructuralAnalysisApi;
using BeamOs.Common.Api;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.ApiClient;

public class InProcessApiClient : DispatchProxy
{
    private IServiceProvider? serviceProvider;

    public InProcessApiClient() { }

    public static IStructuralAnalysisApiAlphaClient Create(IServiceProvider serviceProvider)
    {
        var proxyInterface = Create<IStructuralAnalysisApiAlphaClient, InProcessApiClient>();

        var proxy = (InProcessApiClient)proxyInterface;

        proxy.serviceProvider = serviceProvider;

        return proxyInterface;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        var endpointOpenGeneric = typeof(IBeamOsEndpoint<,>);
        var endpointType = endpointOpenGeneric.MakeGenericType(
            args[0].GetType(),
            GetBeamOsGenericTypeArgumentFromTargetMethod(targetMethod)
        );

        // using var scope = this.serviceProvider.CreateScope(); // service provider should already be scoped
        var endpointInstance = (IBeamOsEndpointBase)
            this.serviceProvider.GetRequiredService(endpointType);

        CancellationToken ct;
        if (args.Length > 1)
        {
            ct = (CancellationToken)args[1];
        }
        else
        {
            ct = CancellationToken.None;
        }
        return endpointInstance.ExecuteRequestAsync(args[0], ct);
    }

    private static Type GetBeamOsGenericTypeArgumentFromTargetMethod(MethodInfo targetMethod)
    {
        return targetMethod.ReturnType.GetGenericArguments().First();
    }
}

public class StructuralAnalysisApiAlphaClientFactory(IServiceProvider serviceProvider)
{
    public IStructuralAnalysisApiAlphaClient Create()
    {
        return InProcessApiClient.Create(serviceProvider);
    }
}
