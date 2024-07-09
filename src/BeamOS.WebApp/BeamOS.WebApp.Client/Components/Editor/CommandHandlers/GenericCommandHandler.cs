using BeamOs.Common.Api;
using BeamOs.WebApp.Client.Events.Interfaces;

namespace BeamOS.WebApp.Client.Components.Editor.CommandHandlers;

public class GenericCommandHandlerWithoutHistory(IServiceProvider services)
{
    public async Task<Result> ExecuteCommandWithoutAddingToHistory(
        IClientAction command,
        CancellationToken ct = default
    )
    {
        Type commandType = command.GetType();
        Type commandHandlerType = typeof(IClientCommandHandler<>).MakeGenericType(commandType);

        if (
            services.GetService(commandHandlerType)
            is not IClientCommandHandler clientCommandHandler
        )
        {
            return Result.Failure(BeamOsError.ServiceNotFound(commandHandlerType.FullName));
        }

        return await clientCommandHandler.ExecuteAsync(command, ct);
    }
}
