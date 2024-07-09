using BeamOS.WebApp.Client.Components.Editor.CommandHandlers;
using BeamOs.WebApp.Client.Events.Interfaces;

namespace BeamOS.WebApp.Client.State;

public sealed class HistoryManager(GenericCommandHandlerWithoutHistory genericCommandHandler)
{
    private readonly LinkedList<IClientActionUndoable> undoActions = new();
    private readonly LinkedList<IClientActionUndoable> redoActions = new();
    private readonly int itemLimit = 50;

    public async Task UndoLast()
    {
        if (this.undoActions.FirstOrDefault() is not IClientActionUndoable undoable)
        {
            // no undo history
            return;
        }

        await genericCommandHandler.ExecuteCommandWithoutAddingToHistory(
            undoable.GetUndoAction(ClientActionSource.CSharp)
        );
    }

    public async Task Redo()
    {
        if (this.redoActions.FirstOrDefault() is not IClientActionUndoable undoable)
        {
            // no redo history
            return;
        }

        await genericCommandHandler.ExecuteCommandWithoutAddingToHistory(
            undoable.WithSource(ClientActionSource.CSharp)
        );
    }

    public void AddItem(IClientActionUndoable clientEvent)
    {
        if (clientEvent.Id == this.undoActions.FirstOrDefault()?.Id)
        {
            // this is undo event
            this.undoActions.RemoveFirst();
            this.redoActions.AddFirst(clientEvent);
        }
        else if (clientEvent.Id == this.redoActions.FirstOrDefault()?.Id)
        {
            // this is redo event
            this.redoActions.RemoveFirst();
            this.undoActions.AddFirst(clientEvent);
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
