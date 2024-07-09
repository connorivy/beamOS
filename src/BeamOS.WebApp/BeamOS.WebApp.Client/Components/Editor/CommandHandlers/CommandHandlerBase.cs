using BeamOs.Common.Api;
using BeamOs.Common.Application.Interfaces;
using BeamOs.WebApp.Client.Events.Interfaces;
using BeamOS.WebApp.Client.State;

namespace BeamOS.WebApp.Client.Components.Editor.CommandHandlers;

public abstract class CommandHandlerBase<TCommand>
    : ICommandHandler<TCommand, Result>,
        IClientCommandHandler<TCommand>
    where TCommand : IClientAction
{
    private readonly HistoryManager historyManager;
    protected virtual bool SkipAddingToHistory { get; }

    protected CommandHandlerBase(HistoryManager historyManager)
    {
        this.historyManager = historyManager;
    }

    public async Task<Result> ExecuteAsync(TCommand command, CancellationToken ct = default)
    {
        Result reponse = await this.ExecuteCommandAsync(command, ct);

        if (command is IClientActionUndoable clientEvent && !this.SkipAddingToHistory)
        {
            this.historyManager.AddItem(clientEvent);
        }

        return reponse;
    }

    protected abstract Task<Result> ExecuteCommandAsync(
        TCommand command,
        CancellationToken ct = default
    );

    public Task<Result> ExecuteAsync(IClientAction command, CancellationToken ct = default) =>
        this.ExecuteAsync((TCommand)command, ct);
}

public interface IClientCommandHandler<TCommand> : IClientCommandHandler { }

public interface IClientCommandHandler
{
    public Task<Result> ExecuteAsync(IClientAction command, CancellationToken ct = default);
}