using System.Reflection;
using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.Common.Api;
using BeamOS.WebApp.Client.Components.Editor;
using BeamOS.WebApp.Client.Components.Editor.CommandHandlers;
using Microsoft.JSInterop;

namespace BeamOS.WebApp.Client;

public class EditorApiProxy : DispatchProxy, IAsyncDisposable
{
    private IJSObjectReference? editorReference;
    private DotNetObjectReference<IEditorEventsApi>? dotNetObjectReference;

    public static async Task<IEditorApiAlpha> Create(
        IJSRuntime js,
        IEditorEventsApi editorEventsApi,
        ChangeComponentStateCommandHandler<EditorComponentState> changeComponentStateCommandHandler,
        string canvasId,
        bool isReadOnly
    )
    {
        var proxyInterface = Create<IEditorApiAlpha, EditorApiProxy>();
        await changeComponentStateCommandHandler.ExecuteAsync(
            new(canvasId, state => state with { EditorApi = proxyInterface })
        );

        var proxy = proxyInterface as EditorApiProxy ?? throw new Exception();

        proxy.dotNetObjectReference = DotNetObjectReference.Create(editorEventsApi);

        // WARNING : the string "createEditorFromId" must match the string in index.js
        proxy.editorReference = await js.InvokeAsync<IJSObjectReference>(
            "createEditorFromId",
            canvasId,
            proxy.dotNetObjectReference,
            isReadOnly
        );

        return proxyInterface;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (args is not null && args.Length > 0 && args.Last() is CancellationToken)
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
            try
            {
                await this.editorReference.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // okay to swallow this exception
            }
        }
    }
}

public class EditorApiProxyFactory(
    IJSRuntime js,
    EditorEventsApi editorEventsApi,
    ChangeComponentStateCommandHandler<EditorComponentState> changeComponentStateCommandHandler
)
{
    public async Task<IEditorApiAlpha> Create(string canvasId, bool isReadOnly)
    {
        return await EditorApiProxy.Create(
            js,
            editorEventsApi,
            changeComponentStateCommandHandler,
            canvasId,
            isReadOnly
        );
    }
}
