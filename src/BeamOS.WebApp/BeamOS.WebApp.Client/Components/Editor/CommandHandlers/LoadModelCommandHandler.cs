using BeamOs.ApiClient;
using BeamOs.Common.Api;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOS.WebApp.Client.Components.Editor.Commands;
using BeamOS.WebApp.Client.State;

namespace BeamOS.WebApp.Client.Components.Editor.CommandHandlers;

public class LoadModelCommandHandler(
    IApiAlphaClient apiAlphaClient,
    HistoryManager historyManager,
    AddEntityContractToEditorCommandHandler addEntityContractToEditorCommandHandler
) : CommandHandlerBase<LoadModelCommand>(historyManager)
{
    protected override async Task<Result> ExecuteCommandAsync(
        LoadModelCommand command,
        CancellationToken ct = default
    )
    {
        ModelResponse response = await apiAlphaClient.GetModelAsync(command.ModelId, null, ct);

        await addEntityContractToEditorCommandHandler.ExecuteAsync(
            new(command.CanvasId, response),
            ct
        );

        return Result.Success();
    }
}
