using BeamOs.WebApp.EditorEvents;
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

        IUndoable undoAction;
        if (undoable is IEditorAction)
        {
            // todo: I know it's ugly, but how to do this otherwise?
            undoAction = CreateUndoActionForEditorAction((dynamic)undoable.GetUndoAction());
        }
        else
        {
            undoAction = CreateUndoAction((dynamic)undoable.GetUndoAction());
        }

        dispatcher.Dispatch(undoAction);
    }

    public IUndoable CreateUndoAction<T>(T undoable)
        where T : struct, IUndoable
    {
        return undoable with { IsUndoAction = true, IsRedoAction = false };
    }

    public IUndoable CreateUndoActionForEditorAction<T>(T undoable)
        where T : struct, IUndoable, IEditorAction
    {
        return undoable with
        {
            IsUndoAction = true,
            IsRedoAction = false,
            UiAlreadyUpdated = false
        };
    }

    public IUndoable CreateRedoAction<T>(T undoable)
        where T : struct, IUndoable
    {
        return undoable with { IsUndoAction = false, IsRedoAction = true, };
    }

    public IUndoable CreateRedoActionForEditorAction<T>(T undoable)
        where T : struct, IUndoable, IEditorAction
    {
        return undoable with
        {
            IsUndoAction = false,
            IsRedoAction = true,
            UiAlreadyUpdated = false
        };
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

        IUndoable redoAction;
        if (undoable is IEditorAction)
        {
            // todo: I know it's ugly, but how to do this otherwise?
            redoAction = CreateRedoActionForEditorAction((dynamic)undoable);
        }
        else
        {
            redoAction = CreateRedoAction((dynamic)undoable);
        }

        dispatcher.Dispatch(redoAction);
    }

    public void AddItem(IUndoable action)
    {
        // don't add the item to the undo actions if this was an action being done
        if (action.IsUndoAction || action.IsRedoAction)
        {
            return;
        }

        this.undoActions.AddFirst(action);

        if (this.redoActions.Count > 0)
        {
            // doing a new action will clear the list of redo actions
            this.redoActions.Clear();
        }
    }
}
