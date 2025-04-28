using BeamOs.Common.Contracts;
using BeamOs.WebApp.EditorCommands.Interfaces;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.Common;

public abstract partial class CommandHandlerBase<TCommand, TResponse>(
    ISnackbar snackbar,
    ILogger logger
)
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
            LogCommandFailed(logger, typeof(TCommand), response.Error.Description);
            snackbar.Add(response.Error.Description, Severity.Error);
        }

        return response;
    }

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Command of type {CommandType} failed with error: {Error}"
    )]
    private static partial void LogCommandFailed(ILogger logger, Type CommandType, string Error);

    protected virtual void PostProcess(TCommand command, Result<TResponse> response) { }

    protected abstract Task<Result<TResponse>> ExecuteCommandAsync(
        TCommand command,
        CancellationToken ct = default
    );
}

public interface IClientCommandHandler
{
    public Task ExecuteAsync(IBeamOsClientCommand command, CancellationToken ct = default);
}

public interface IClientCommandHandler<TCommand> : IClientCommandHandler
    where TCommand : IBeamOsClientCommand
{
    public Task ExecuteAsync(TCommand command, CancellationToken ct = default);
}

public abstract partial class ClientCommandHandlerBase<TCommand, TServerResponse>(
    ILogger logger,
    ISnackbar snackbar
) : IClientCommandHandler<TCommand>
    where TCommand : IBeamOsClientCommand
{
    protected ISnackbar Snackbar => snackbar;

    public event EventHandler<bool>? IsLoadingChanged;

    public async Task ExecuteAsync(TCommand command, CancellationToken ct = default)
    {
        Result result;
        try
        {
            result = await this.ExecuteCommandAsync(command, ct);
            if (result.IsError)
            {
                snackbar.Add(result.Error.Description, Severity.Error);
            }
        }
        catch (Exception ex)
        {
            this.CommandFailed(ex, typeof(TCommand));
            snackbar.Add(ex.Message, Severity.Error);
        }
    }

    protected async Task<Result> ExecuteCommandAsync(
        TCommand command,
        CancellationToken ct = default
    )
    {
        if (!command.HandledByEditor)
        {
            var result = await this.UpdateEditor(command);
            if (result.IsError)
            {
                this.FailedToCreateNode(result.Error);
                return result.Error;
            }
        }

        Result<TServerResponse> serverResponse;
        // if (!command.HandledByServer)
        // {
        try
        {
            IsLoadingChanged?.Invoke(this, true);
            serverResponse = await this.UpdateServer(command, ct);
        }
        catch (Exception ex)
        {
            this.FailedToUpdateServer(ex);
            serverResponse = BeamOsError.Failure(description: ex.Message);
        }
        finally
        {
            IsLoadingChanged?.Invoke(this, false);
        }

        if (serverResponse.IsError)
        {
            snackbar.Add(serverResponse.Error.Description, Severity.Error);
        }
        // }

        await this.UpdateEditorAfterServerResponse(command, serverResponse);
        if (serverResponse.IsError)
        {
            return serverResponse.Error;
        }

        if (!command.HandledByBlazor)
        {
            var result = await this.UpdateClient(command, serverResponse);
            if (result.IsError)
            {
                this.FailedToUpdateClient(result.Error);
                return result.Error;
            }
        }

        return Result.Success;
    }

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Executing command of type {commandType} failed"
    )]
    private partial void CommandFailed(Exception ex, Type commandType);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to create node in editor with error: `{error}`"
    )]
    private partial void FailedToCreateNode(BeamOsError error);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Updating server failed with unexpected exception"
    )]
    private partial void FailedToUpdateServer(Exception ex);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to update client with error: `{error}`"
    )]
    private partial void FailedToUpdateClient(BeamOsError error);

    protected virtual ValueTask<Result> UpdateEditor(TCommand command) =>
        ValueTask.FromResult(Result.Success);

    protected virtual ValueTask<Result> UpdateEditorAfterServerResponse(
        TCommand command,
        Result<TServerResponse> serverResponse
    ) => ValueTask.FromResult(Result.Success);

    protected virtual ValueTask<Result<TServerResponse>> UpdateServer(
        TCommand command,
        CancellationToken ct = default
    ) => ValueTask.FromResult(Result<TServerResponse>.Success());

    protected virtual ValueTask<Result> UpdateClient(
        TCommand command,
        Result<TServerResponse> serverResponse
    ) => ValueTask.FromResult(Result.Success);

    public Task ExecuteAsync(IBeamOsClientCommand command, CancellationToken ct = default) =>
        this.ExecuteAsync((TCommand)command, ct);
}

public abstract class SimpleCommandHandlerBase<TSimpleCommand, TCommand, TServerResponse>(
    ClientCommandHandlerBase<TCommand, TServerResponse> clientCommandHandler
)
    where TCommand : IBeamOsClientCommand
{
    public async Task ExecuteAsync(TSimpleCommand command, CancellationToken ct = default) =>
        await clientCommandHandler.ExecuteAsync(this.CreateCommand(command), ct);

    protected abstract TCommand CreateCommand(TSimpleCommand simpleCommand);
}
