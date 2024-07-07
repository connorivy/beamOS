using BeamOs.IntegrationEvents.Common;
using BeamOS.WebApp.Client.Features.Common.Flux;
using Fluxor;

namespace BeamOS.WebApp.Client.State;

public sealed class HistoryManager(IDispatcher dispatcher)
{
    private readonly LinkedList<IUndoable> undoActions = new();
    private readonly LinkedList<IUndoable> redoActions = new();
    private readonly int itemLimit = 50;

    public void UndoLast()
    {
        if (this.undoActions.FirstOrDefault() is not IUndoable undoable)
        {
            // no undo history
            return;
        }

        this.undoActions.RemoveFirst();
        this.redoActions.AddFirst(undoable);

        dispatcher.Dispatch(new HistoryEvent(undoable.GetUndoAction()));
    }

    public void Redo()
    {
        if (this.redoActions.FirstOrDefault() is not IUndoable undoable)
        {
            // no redo history
            return;
        }
        this.redoActions.RemoveFirst();
        this.undoActions.AddFirst(undoable);

        dispatcher.Dispatch(new HistoryEvent(undoable));
    }

    public void AddItem(IIntegrationEventWrapper integrationEventWrapper)
    {
        // don't add the item to the undo actions if this was an action being done
        if (
            integrationEventWrapper is HistoryEvent
            || integrationEventWrapper.IntegrationEvent is not IUndoable undoable
        )
        {
            return;
        }

        this.undoActions.AddFirst(undoable);

        if (this.redoActions.Count > 0)
        {
            // doing a new action will clear the list of redo actions
            this.redoActions.Clear();
        }
    }

    public void Clear()
    {
        this.undoActions.Clear();
        this.redoActions.Clear();
    }
}
