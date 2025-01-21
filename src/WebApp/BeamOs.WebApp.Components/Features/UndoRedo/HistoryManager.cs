using BeamOs.WebApp.EditorCommands.Interfaces;
using Fluxor;

namespace BeamOs.WebApp.Components.Features.UndoRedo;

public sealed class HistoryManager(IDispatcher dispatcher)
{
    private readonly LinkedList<IBeamOsClientCommand> undoActions = new();
    private readonly LinkedList<IBeamOsClientCommand> redoActions = new();
    private readonly int itemLimit = 50;

    public void UndoLast()
    {
        if (this.undoActions.FirstOrDefault() is not IBeamOsClientCommand undoable)
        {
            // no undo history
            return;
        }

        dispatcher.Dispatch(undoable.GetUndoCommand(new(false, false)));

        this.undoActions.RemoveFirst();
        this.redoActions.AddFirst(undoable);
    }

    public void Redo()
    {
        if (this.redoActions.FirstOrDefault() is not IBeamOsClientCommand undoable)
        {
            // no redo history
            return;
        }

        dispatcher.Dispatch(undoable.WithArgs(new(false, false)));

        this.redoActions.RemoveFirst();
        this.undoActions.AddFirst(undoable);
    }

    public void AddItem(IBeamOsClientCommand clientEvent)
    {
        if (clientEvent.Id == this.undoActions.FirstOrDefault()?.Id)
        {
            // this is undo event
        }
        else if (clientEvent.Id == this.redoActions.FirstOrDefault()?.Id)
        {
            // this is redo event
        }
        else
        {
            // this is brand new event
            this.undoActions.AddFirst(clientEvent);

            if (this.redoActions.Count > 0)
            {
                // doing a new action will clear the list of redo actions
                this.redoActions.Clear();
            }
        }
    }

    public void Clear()
    {
        this.undoActions.Clear();
        this.redoActions.Clear();
    }
}
