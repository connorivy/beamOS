using BeamOs.Common.Api;
using BeamOs.Common.Application.Interfaces;
using BeamOs.WebApp.Client.Events.Interfaces;
using BeamOS.WebApp.Client.State;

namespace BeamOS.WebApp.Client.Components.Editor.CommandHandlers;

public abstract class CommandHandlerBase<TCommand>(HistoryManager historyManager)
    : ICommandHandler<TCommand, Result>,
        IClientCommandHandler<TCommand>
    where TCommand : IClientCommand
{
    public async Task<Result> ExecuteAsync(TCommand command, CancellationToken ct = default)
    {
        Result reponse = await this.ExecuteCommandAsync(command, ct);

        this.PostProcess(command);

        return reponse;
    }

    protected virtual void PostProcess(TCommand command)
    {
        if (command is IClientCommandUndoable clientEvent)
        {
            historyManager.AddItem(clientEvent);
        }
    }

    protected abstract Task<Result> ExecuteCommandAsync(
        TCommand command,
        CancellationToken ct = default
    );

    public Task<Result> ExecuteAsync(IClientCommand command, CancellationToken ct = default) =>
        this.ExecuteAsync((TCommand)command, ct);
}

public interface IClientCommandHandler<TCommand> : IClientCommandHandler { }

public interface IClientCommandHandler
{
    public Task<Result> ExecuteAsync(IClientCommand command, CancellationToken ct = default);
}
