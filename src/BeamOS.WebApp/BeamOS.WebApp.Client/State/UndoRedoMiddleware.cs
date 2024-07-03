using BeamOs.IntegrationEvents.Common;
using Fluxor;

namespace BeamOS.WebApp.Client.State;

public class UndoRedoMiddleware : Middleware
{
    private readonly HistoryManager historyManager;

    public UndoRedoMiddleware(HistoryManager historyManager)
    {
        this.historyManager = historyManager;
    }

    public override void AfterDispatch(object action)
    {
        if (action is IUndoable undoable)
        {
            this.historyManager.AddItem(undoable);
        }
    }
}
