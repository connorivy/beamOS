using BeamOs.WebApp.EditorCommands.Interfaces;
using Fluxor;

namespace BeamOs.WebApp.Components.Features.UndoRedo;

public class HistoryMiddleware(HistoryManager historyManager) : Middleware
{
    public override void AfterDispatch(object action)
    {
        if (action is IBeamOsClientCommand clientCommand)
        {
            historyManager.AddItem(clientCommand);
        }
    }
}
