using Fluxor;

namespace BeamOS.WebApp.Client.State;

public class UndoRedoMiddleware : Middleware
{
    private readonly HistoryDeque historyDeque;

    public UndoRedoMiddleware(HistoryDeque historyDeque)
    {
        this.historyDeque = historyDeque;
    }

    public override void AfterDispatch(object action)
    {
        this.historyDeque.PushFirst(action);
    }
}
