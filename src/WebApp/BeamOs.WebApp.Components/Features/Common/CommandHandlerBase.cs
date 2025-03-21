using BeamOs.Common.Contracts;
using BeamOs.WebApp.EditorCommands.Interfaces;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.Common;

public abstract class CommandHandlerBase<TCommand, TResponse>(ISnackbar snackbar)
//: ICommandHandler<TCommand, Result>,
//IClientCommandHandler<TCommand>
//where TCommand : IClientCommand
{
    protected ISnackbar Snackbar => snackbar;

    public event EventHandler<bool>? IsLoadingChanged;

    public async Task<Result<TResponse>> ExecuteAsync(
        TCommand command,
        CancellationToken ct = default
    )
    {
        Result<TResponse> response;
        try
        {
            IsLoadingChanged?.Invoke(this, true);
            response = await this.ExecuteCommandAsync(command, ct);
        }
        catch (Exception ex)
        {
            response = BeamOsError.Failure(description: ex.Message);
        }
        finally
        {
            IsLoadingChanged?.Invoke(this, false);
        }

        this.PostProcess(command, response);

        if (response.IsError)
        {
            snackbar.Add(response.Error.Description, Severity.Error);
        }

        return response;
    }

    protected virtual void PostProcess(TCommand command, Result<TResponse> response) { }

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

public interface IBeamOsClientCommandHandler<TCommand> : IBeamOsClientCommandHandler { }

public interface IBeamOsClientCommandHandler
{
    public Task<Result> ExecuteAsync(IBeamOsClientCommand command, CancellationToken ct = default);
}

// public abstract class EditorCommandHandlerBase<TCommand, TResponse, TServerResponse>(ISnackbar snackbar)
//     : CommandHandlerBase<TCommand, TResponse>(snackbar),
//         IBeamOsClientCommandHandler<TCommand>
//     where TCommand : IBeamOsClientCommand
// {
//     protected override async Task<Result<TResponse>> ExecuteCommandAsync(
//         TCommand command,
//         CancellationToken ct = default
//     )
//     {
//         Result<TServerResponse>? result = null;
//         if (!command.HandledByServer)
//         {
//             result = await this.UpdateServer(command, ct);
//             if (result.IsError)
//             {
//                 return result.Error;
//             }
//         }

//         if (!command.HandledByBlazor)
//         {
//             await this.UpdateClient(command, result!);
//         }

//         if (!command.HandledByEditor)
//         {
//             await this.UpdateEditor(command, result!);
//         }
//     }

//     protected abstract Task<Result<TServerResponse>> UpdateServer(
//         TCommand command,
//         CancellationToken ct = default
//     );

//     protected virtual ValueTask UpdateClient(TCommand command, Result<TServerResponse>? response) =>
//         ValueTask.CompletedTask;

//     protected virtual ValueTask UpdateEditor(TCommand command, Result<TServerResponse>? response) =>
//         ValueTask.CompletedTask;

//     public async Task<Result> ExecuteAsync(
//         IBeamOsClientCommand command,
//         CancellationToken ct = default
//     ) => await this.ExecuteAsync((TCommand)command, ct);
// }
