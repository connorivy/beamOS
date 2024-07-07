using BeamOS.WebApp.Client.Features.Common.Flux;
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
        if (action is IIntegrationEventWrapper integrationEventWrapper)
        {
            this.historyManager.AddItem(integrationEventWrapper);
        }
    }
}
