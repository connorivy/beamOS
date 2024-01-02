using System.Reflection;
using BeamOS.PhysicalModel.Contracts.Common;
using BeamOS.PhysicalModel.Contracts.Node;
using BeamOS.WebApp.EditorApi;
using Microsoft.JSInterop;

namespace BeamOS.WebApp.Client;

public class EditorApiProxy : DispatchProxy
{
    private IJSRuntime? js;

    public static IEditorApiAlpha Create(IJSRuntime js)
    {
        var proxyInterface = Create<IEditorApiAlpha, EditorApiProxy>();
        var proxy = proxyInterface as EditorApiProxy ?? throw new Exception();
        proxy.js = js;
        return proxyInterface;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (args is not null && args.Length > 0 && args.Last() is CancellationToken ct)
        {
            args = args[..^1];
        }

        if (this.js is null)
        {
            throw new Exception("Must use factory method to Create EditorApiProxy");
        }

        return this.js
            .InvokeAsync<string>(
                $"beamOsEditor.api.{GetTsMethodName(targetMethod?.Name ?? throw new ArgumentNullException())}",
                args
            )
            .AsTask();
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