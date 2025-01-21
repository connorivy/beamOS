using Microsoft.AspNetCore.Components.Web;

namespace BeamOs.WebApp.Components.Features.UndoRedo;

public sealed class UndoRedoFunctionality : IDisposable
{
    private readonly HistoryManager history;
    private readonly EventHandler<KeyboardEventArgs> eventHandler;

    public UndoRedoFunctionality(HistoryManager history)
    {
        this.eventHandler = (s, e) => this.DocumentEvents_OnKeyDown(e);
        DocumentEvents.OnKeyDown += this.eventHandler;
        this.history = history;
    }

    public static IDisposable SubscribeToUndoRedo(HistoryManager historyManager)
    {
        return new UndoRedoFunctionality(historyManager);
    }

    private void DocumentEvents_OnKeyDown(KeyboardEventArgs e)
    {
        if (!e.CtrlKey)
        {
            return;
        }

        switch (e.Code)
        {
            // y
            case "89":
                this.history.Redo();
                break;

            // z
            case "90":
                this.history.UndoLast();
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
