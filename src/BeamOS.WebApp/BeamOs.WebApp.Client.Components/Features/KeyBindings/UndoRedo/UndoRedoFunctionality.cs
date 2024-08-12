using BeamOs.WebApp.Client.Components.State;
using Microsoft.AspNetCore.Components.Web;

namespace BeamOs.WebApp.Client.Components.Features.KeyBindings.UndoRedo;

public sealed class UndoRedoFunctionality : IDisposable
{
    private readonly HistoryManager history;
    private readonly EventHandler<KeyboardEventArgs> eventHandler;

    public UndoRedoFunctionality(HistoryManager history)
    {
        this.eventHandler = async (s, e) => await this.DocumentEvents_OnKeyDown(e);
        DocumentEvents.OnKeyDown += this.eventHandler;
        this.history = history;
    }

    private async Task DocumentEvents_OnKeyDown(KeyboardEventArgs e)
    {
        if (!e.CtrlKey)
        {
            return;
        }

        switch (e.Code)
        {
            // y
            case "89":
                await this.history.Redo();
                break;

            // z
            case "90":
                await this.history.UndoLast();
                break;

            default:
                break;
        }
    }

    public void Dispose()
    {
        DocumentEvents.OnKeyDown -= this.eventHandler;
    }
}
