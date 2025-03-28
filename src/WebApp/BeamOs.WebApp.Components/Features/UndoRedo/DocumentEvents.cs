using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BeamOs.WebApp.Components.Features.UndoRedo;

public static class DocumentEvents
{
    public static event EventHandler<KeyboardEventArgs>? OnKeyDown;

    [JSInvokable]
    public static void FireOnKeyDownEvent(KeyboardEventArgs e)
    {
        OnKeyDown?.Invoke(typeof(DocumentEvents), e);
    }
}
