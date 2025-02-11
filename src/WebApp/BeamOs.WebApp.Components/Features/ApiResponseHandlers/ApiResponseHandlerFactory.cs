using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.WebApp.Components.Features.Editor;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.WebApp.Components.Features.ApiResponseHandlers;

public class ApiResponseHandlerFactory
{
    //private readonly Dictionary<Type, IApiResponseHandler> apiResponseHandlerDict;
    private readonly IServiceProvider serviceProvider;

    public ApiResponseHandlerFactory(
        //IEnumerable<IApiResponseHandler> apiResponseHandlers,
        IServiceProvider serviceProvider
    )
    {
        //apiResponseHandlerDict = apiResponseHandlers
        //    .GroupBy(
        //        handler =>
        //            handler
        //                .GetType()
        //                .GetMethod(nameof(IApiResponseHandler<>.HandleResponseAsync))
        //                .GetParameters()[1]
        //                .ParameterType
        //    )
        //    .ToDictionary(group => group.Key, group => group.ToArray());
        //this.apiResponseHandlerDict = apiResponseHandlers.ToDictionary(
        //    handler =>
        //        handler
        //            .GetType()
        //            .GetMethod(nameof(IApiResponseHandler<>.HandleResponseAsync))
        //            .GetParameters()[1]
        //            .ParameterType,
        //    handler => handler
        //);
        this.serviceProvider = serviceProvider;
    }

    public IApiResponseHandler CreateHandler(object responseObject)
    {
        var interfaceType = typeof(IApiResponseHandler<>).MakeGenericType(responseObject.GetType());
        return (IApiResponseHandler)this.serviceProvider.GetRequiredService(interfaceType);
        //if (!apiResponseHandlerDict.TryGetValue(responseObject.GetType(), out var handler))
        //{
        //    throw new InvalidOperationException(
        //        $"No handler found for response object of type {responseObject.GetType()}"
        //    );
        //}

        //return handler;
    }
}

public interface IApiResponseHandler
{
    public ValueTask<Result> HandleResponseObjectAsync(string http, object responseObject);
}

public interface IApiResponseHandler<T> : IApiResponseHandler
{
    public ValueTask<Result> HandleResponseAsync(string http, T responseObject);

    ValueTask<Result> IApiResponseHandler.HandleResponseObjectAsync(
        string http,
        object responseObject
    ) => this.HandleResponseAsync(http, (T)responseObject);
}

//public class DiagramResponseHandler(IDispatcher dispatcher) : IApiResponseHandler<DiagramResponse>
//{
//    public ValueTask<Result> HandleResponseAsync(string http, DiagramResponse responseObject)
//    {
//        if (http == Http.Post)
//        {
//            dispatcher.Dispatch(
//                new ShearDiagramResponsesCreated() { ShearDiagrams = responseObject.ShearDiagrams }
//            );
//            dispatcher.Dispatch(
//                new MomentDiagramResponsesCreated()
//                {
//                    MomentDiagrams = responseObject.MomentDiagrams
//                }
//            );
//            dispatcher.Dispatch(
//                new DeflectionDiagramResponsesCreated()
//                {
//                    DeflectionDiagrams = responseObject.DeflectionDiagrams
//                }
//            );
//        }
//        return ValueTask.FromResult(Result.Success);
//    }
//}

public readonly record struct AnalyticalResultsCreated
{
    public required AnalyticalResultsResponse AnalyticalResults { get; init; }
}
