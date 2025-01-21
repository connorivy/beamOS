using BeamOs.Common.Contracts;
using BeamOs.WebApp.EditorCommands.Interfaces;

namespace BeamOs.WebApp.Components.Features.Common;

public abstract class CommandHandlerBase<TCommand>
//: ICommandHandler<TCommand, Result>,
//IClientCommandHandler<TCommand>
//where TCommand : IClientCommand
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

public abstract class CommandHandlerSyncBase<TCommand> : CommandHandlerBase<TCommand>
    where TCommand : IClientCommand
{
    protected sealed override Task<Result> ExecuteCommandAsync(
        TCommand command,
        CancellationToken ct = default
    ) => Task.FromResult(this.ExecuteCommandSync(command));

    public Result ExecuteSync(TCommand command)
    {
        Result response = this.ExecuteCommandSync(command);

        this.PostProcess(command);

        return response;
    }

    protected abstract Result ExecuteCommandSync(TCommand command);
}

public interface IClientCommandHandler<TCommand> : IClientCommandHandler { }

public interface IClientCommandHandler
{
    public Task<Result> ExecuteAsync(IClientCommand command, CancellationToken ct = default);
}
