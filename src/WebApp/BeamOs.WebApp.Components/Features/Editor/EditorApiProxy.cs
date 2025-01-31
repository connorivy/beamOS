using System.Reflection;
using System.Text;
using BeamOs.CodeGen.EditorApi;
using BeamOs.Common.Contracts;
using Microsoft.JSInterop;

namespace BeamOs.WebApp.Components.Features.Editor;

public class EditorApiProxy : DispatchProxy, IAsyncDisposable
{
    private IJSObjectReference? editorReference;

    private DotNetObjectReference<IEditorEventsApi>? dotNetObjectReference;

    public static async Task<IEditorApiAlpha> Create(
        IJSRuntime js,
        IEditorEventsApi editorEventsApi,
        //ChangeComponentStateCommandHandler<EditorComponentState> changeComponentStateCommandHandler,
        string canvasId,
        bool isReadOnly
    )
    {
        var proxyInterface = Create<IEditorApiAlpha, EditorApiProxy>();
        //await changeComponentStateCommandHandler.ExecuteAsync(
        //    new(canvasId, state => state with { EditorApi = proxyInterface })
        //);

        var proxy = proxyInterface as EditorApiProxy ?? throw new Exception();

        proxy.dotNetObjectReference = DotNetObjectReference.Create(editorEventsApi);

        // for cases when editor page is directly navigated to, you may need to wait a bit
        // for the editor api to be loaded from npm or local
        bool importedEditorModule = false;
        while (!importedEditorModule)
        {
            importedEditorModule = await js.InvokeAsync<bool>(
                "window.hasOwnProperty",
                "createEditorFromId"
            );
        }

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
            tsMethodName = tsMethodName[..^asyncSuffix.Length];
        }

        if (tsMethodName.Length > 0 && char.IsUpper(tsMethodName.First()))
        {
            tsMethodName = char.ToLower(tsMethodName[0]) + tsMethodName[1..];
        }

        return tsMethodName;
    }

    public async ValueTask DisposeAsync()
    {
        //this.dotNetObjectReference?.Dispose();
        if (this.editorReference is not null)
        {
            await this.editorReference.InvokeVoidAsync("dispose");
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
