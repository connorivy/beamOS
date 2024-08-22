using BeamOs.Common.Api;
using BeamOs.Common.Application.Interfaces;
using BeamOs.WebApp.Client.Events.Interfaces;

namespace BeamOs.WebApp.Client.Components.Components.Editor.CommandHandlers;

public abstract class CommandHandlerBase<TCommand>
    : ICommandHandler<TCommand, Result>,
        IClientCommandHandler<TCommand>
    where TCommand : IClientCommand
{
    public async Task<Result> ExecuteAsync(TCommand command, CancellationToken ct = default)
    {
        Result response = await this.ExecuteCommandAsync(command, ct);

        this.PostProcess(command);

        return response;
    }

    protected virtual void PostProcess(TCommand command) { }

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
