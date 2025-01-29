using BeamOs.Common.Contracts;
using BeamOs.WebApp.EditorCommands.Interfaces;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.Common;

public abstract class CommandHandlerBase<TCommand, TResponse>(ISnackbar snackbar)
//: ICommandHandler<TCommand, Result>,
//IClientCommandHandler<TCommand>
//where TCommand : IClientCommand
{
    public async Task<Result<TResponse>> ExecuteAsync(
        TCommand command,
        CancellationToken ct = default
    )
    {
        Result<TResponse> response;
        try
        {
            response = await this.ExecuteCommandAsync(command, ct);

            this.PostProcess(command);
        }
        catch (Exception ex)
        {
            response = BeamOsError.Failure(description: ex.Message);
        }

        if (response.IsError)
        {
            snackbar.Add(response.Error.Description, Severity.Error);
        }

        return response;
    }

    protected virtual void PostProcess(TCommand command) { }

    protected abstract Task<Result<TResponse>> ExecuteCommandAsync(
        TCommand command,
        CancellationToken ct = default
    );
}

//public abstract class CommandHandlerSyncBase<TCommand> : CommandHandlerBase<TCommand>
//    where TCommand : IClientCommand
//{
//    protected sealed override Task<Result> ExecuteCommandAsync(
//        TCommand command,
//        CancellationToken ct = default
//    ) => Task.FromResult(this.ExecuteCommandSync(command));

//    public Result ExecuteSync(TCommand command)
//    {
//        Result response = this.ExecuteCommandSync(command);

//        this.PostProcess(command);

//        return response;
//    }

//    protected abstract Result ExecuteCommandSync(TCommand command);
//}

public interface IClientCommandHandler<TCommand> : IClientCommandHandler { }

public interface IClientCommandHandler
{
    public Task<Result> ExecuteAsync(IClientCommand command, CancellationToken ct = default);
}
