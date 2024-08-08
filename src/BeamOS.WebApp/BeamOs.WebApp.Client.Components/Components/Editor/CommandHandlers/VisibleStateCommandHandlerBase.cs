using BeamOs.WebApp.Client.Components.State;
using BeamOs.WebApp.Client.Events.Interfaces;

namespace BeamOs.WebApp.Client.Components.Components.Editor.CommandHandlers;

public abstract class VisibleStateCommandHandlerBase<TCommand>(HistoryManager historyManager)
    : CommandHandlerBase<TCommand>(historyManager)
    where TCommand : IClientCommand
{
    protected override void PostProcess(TCommand command)
    {
        EventEmitter.EmitVisibleStateChangedEvent();
        base.PostProcess(command);
    }
}

public static class EventEmitter
{
    public static event EventHandler? VisibleStateChanged;

    public static void EmitVisibleStateChangedEvent()
    {
        VisibleStateChanged?.Invoke(typeof(EventEmitter), EventArgs.Empty);
    }
}
