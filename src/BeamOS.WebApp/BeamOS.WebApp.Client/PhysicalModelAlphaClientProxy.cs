using BeamOS.PhysicalModel.Client;
using Microsoft.JSInterop;

namespace BeamOS.WebApp.Client;

public interface IPhysicalModelAlphaClientWithEditor
{
    public Task ExecuteApiMethod<TRequest, TResponse>(
        Func<IPhysicalModelAlphaClient, Func<TRequest, Task<TResponse>>> action,
        TRequest request,
        string csMethodName
    );
}

public class PhysicalModelAlphaClientProxy(
    IJSRuntime js,
    IPhysicalModelAlphaClient physicalModelAlphaClient
) : IPhysicalModelAlphaClientWithEditor
{
    public async Task ExecuteApiMethod<TRequest, TResponse>(
        Func<IPhysicalModelAlphaClient, Func<TRequest, Task<TResponse>>> action,
        TRequest request,
        string csMethodName
    )
    {
        Func<TRequest, Task<TResponse>> clientInjected = action(physicalModelAlphaClient);
        TResponse response = await clientInjected(request);
        if (response != null)
        {
            await js.InvokeVoidAsync($"beamOsEditor.api.{GetTsMethodName(csMethodName)}", request);

            return;
        }
        throw new Exception(); // TODO
    }

    private static string GetTsMethodName(string csMethodName)
    {
        string tsMethodName = csMethodName;
        const string asyncSuffix = "Async";
        if (tsMethodName.EndsWith(asyncSuffix))
        {
            tsMethodName = tsMethodName.Substring(0, tsMethodName.Length - asyncSuffix.Length);
        }

        if (tsMethodName.Length > 0 && char.IsUpper(tsMethodName.First()))
        {
            tsMethodName = char.ToLower(tsMethodName[0]) + tsMethodName.Substring(1);
        }

        return tsMethodName;
    }
}

//public interface IPhysicalModelAlphaClientWithEditor : IPhysicalModelAlphaClient { }

//public class PhysicalModelAlphaClientProxy(
//    IJSRuntime js,
//    IPhysicalModelAlphaClient physicalModelAlphaClient
//) : IPhysicalModelAlphaClientWithEditor
//{
//    private async Task<TResponse> ValidateAndCallClient<TRequest, TResponse>(
//        TRequest request,
//        TResponse? response,
//        [CallerMemberName] string methodName = null!
//    )
//    {
//        if (response != null)
//        {
//            await js.InvokeVoidAsync($"beamOsEditor.api.{GetTsMethodName(methodName)}", request);

//            return response;
//        }

//        throw new Exception(); // TODO
//    }

//    private static string GetTsMethodName(string csMethodName)
//    {
//        string tsMethodName = csMethodName;
//        const string asyncSuffix = "Async";
//        if (tsMethodName.EndsWith(asyncSuffix))
//        {
//            tsMethodName = tsMethodName.Substring(0, tsMethodName.Length - asyncSuffix.Length);
//        }

//        if (tsMethodName.Length > 0 && char.IsUpper(tsMethodName.First()))
//        {
//            tsMethodName = char.ToLower(tsMethodName[0]) + tsMethodName.Substring(1);
//        }

//        return tsMethodName;
//    }

//    public async Task<Element1DResponse> CreateElement1dAsync(
//        CreateElement1DRequest createElement1DRequest
//    )
//    {
//        var response = await physicalModelAlphaClient.CreateElement1dAsync(createElement1DRequest);
//        return await this.ValidateAndCallClient(createElement1DRequest, response);
//    }

//    public Task<Element1DResponse> CreateElement1dAsync(
//        CreateElement1DRequest createElement1DRequest,
//        CancellationToken cancellationToken
//    ) => throw new NotImplementedException();

//    public async Task<ModelResponse> CreateModelAsync(CreateModelRequest createModelRequest)
//    {
//        var response = await physicalModelAlphaClient.CreateModelAsync(createModelRequest);
//        return await this.ValidateAndCallClient(createModelRequest, response);
//    }

//    public Task<ModelResponse> CreateModelAsync(
//        CreateModelRequest createModelRequest,
//        CancellationToken cancellationToken
//    ) => throw new NotImplementedException();

//    public async Task<NodeResponse> CreateNodeAsync(CreateNodeRequest createNodeRequest)
//    {
//        var response = await physicalModelAlphaClient.CreateNodeAsync(createNodeRequest);
//        return await this.ValidateAndCallClient(createNodeRequest, response);
//    }

//    public Task<NodeResponse> CreateNodeAsync(
//        CreateNodeRequest createNodeRequest,
//        CancellationToken cancellationToken
//    ) => throw new NotImplementedException();

//    public async Task<PointLoadResponse> CreatePointLoadAsync(
//        CreatePointLoadRequest createPointLoadRequest
//    )
//    {
//        var response = await physicalModelAlphaClient.CreatePointLoadAsync(createPointLoadRequest);
//        return await this.ValidateAndCallClient(createPointLoadRequest, response);
//    }

//    public Task<PointLoadResponse> CreatePointLoadAsync(
//        CreatePointLoadRequest createPointLoadRequest,
//        CancellationToken cancellationToken
//    ) => throw new NotImplementedException();

//    public Task<ModelResponse> GetApiModelsAsync(string id, bool? sendEntities)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<ModelResponse> GetApiModelsAsync(
//        string id,
//        bool? sendEntities,
//        CancellationToken cancellationToken
//    ) => throw new NotImplementedException();

//    public Task<ICollection<Element1DResponse>> GetApiModelsElement1DsAsync(
//        string modelId,
//        IEnumerable<string> element1dIds
//    )
//    {
//        throw new NotImplementedException();
//    }

//    public Task<ICollection<Element1DResponse>> GetApiModelsElement1DsAsync(
//        string modelId,
//        IEnumerable<string> element1dIds,
//        CancellationToken cancellationToken
//    ) => throw new NotImplementedException();

//    public Task<Element1DResponse> GetSingleElement1dAsync(string id)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<Element1DResponse> GetSingleElement1dAsync(
//        string id,
//        CancellationToken cancellationToken
//    ) => throw new NotImplementedException();

//    public Task<NodeResponse> GetSingleNodeAsync(string id)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<NodeResponse> GetSingleNodeAsync(string id, CancellationToken cancellationToken) =>
//        throw new NotImplementedException();
//}
