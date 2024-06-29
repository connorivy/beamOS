using System.Reflection;
using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.Common.Api;
using Microsoft.JSInterop;

namespace BeamOS.WebApp.Client;

public class EditorApiProxy : DispatchProxy, IAsyncDisposable
{
    private IJSObjectReference? editorReference;
    private DotNetObjectReference<IEditorEventsApi>? dotNetObjectReference;

    public static async Task<IEditorApiAlpha> Create(
        IJSRuntime js,
        IEditorEventsApi editorEventsApi,
        string canvasId
    )
    {
        var proxyInterface = Create<IEditorApiAlpha, EditorApiProxy>();
        var proxy = proxyInterface as EditorApiProxy ?? throw new Exception();

        proxy.dotNetObjectReference = DotNetObjectReference.Create(editorEventsApi);

        // WARNING : the string "createEditorFromId" must match the string in index.js
        proxy.editorReference = await js.InvokeAsync<IJSObjectReference>(
            "createEditorFromId",
            canvasId,
            proxy.dotNetObjectReference
        );
        return proxyInterface;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (args is not null && args.Length > 0 && args.Last() is CancellationToken ct)
        {
            args = args[..^1];
        }

        if (this.editorReference is null)
        {
            throw new Exception("Must use factory method to Create EditorApiProxy");
        }

        return this.editorReference
            .InvokeAsync<Result>(
                $"api.{GetTsMethodName(targetMethod?.Name ?? throw new ArgumentNullException())}",
                args
            )
            .AsTask();
    }

    private static string GetTsMethodName(string csMethodName)
    {
        var tsMethodName = csMethodName;
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

    public async ValueTask DisposeAsync()
    {
        this.dotNetObjectReference?.Dispose();
        if (this.editorReference is not null)
        {
            await this.editorReference.DisposeAsync();
        }
    }
}

public class EditorApiProxyFactory(IJSRuntime js, EditorEventsApi editorEventsApi)
{
    public async Task<IEditorApiAlpha> Create(string canvasId)
    {
        return await EditorApiProxy.Create(js, editorEventsApi, canvasId);
    }
}
