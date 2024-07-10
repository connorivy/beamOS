using BeamOs.Common.Api;

namespace BeamOS.WebApp.Client.Components.Editor.CommandHandlers;

public abstract class VisibleStateCommandHandlerBase<TCommand>
    : IVisibleStateCommandHandler<TCommand>
{
    public Task<Result> ExecuteCommandAsync(
        TCommand command,
        Action stateCallback,
        CancellationToken ct = default
    ) => throw new NotImplementedException();
}

public interface IVisibleStateCommandHandler<TCommand>
{
    Task<Result> ExecuteCommandAsync(
        TCommand command,
        Action stateCallback,
        CancellationToken ct = default
    );
}

public static class EventEmitter
{
    public static event EventHandler? VisibleStateChanged;

    public void VisibleStateChanged()
    {
        VisibleStateChanged.Raise(this);
    }
}
