using BeamOS.WebApp.Client.State;
using Fluxor;
using Microsoft.AspNetCore.Components.Web;

namespace BeamOS.WebApp.Client.Features.KeyBindings.UndoRedo;

public sealed class UndoRedoFunctionality : IDisposable
{
    private readonly HistoryManager history;

    public UndoRedoFunctionality(HistoryManager history)
    {
        DocumentEvents.OnKeyDown += this.DocumentEvents_OnKeyDown;
        this.history = history;
    }

    private void DocumentEvents_OnKeyDown(object? sender, KeyboardEventArgs e)
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
        DocumentEvents.OnKeyDown -= this.DocumentEvents_OnKeyDown;
    }
}
