using BeamOs.ApiClient;
using BeamOs.Common.Api;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOS.WebApp.Client.Components.Editor.Commands;
using BeamOS.WebApp.Client.State;

namespace BeamOS.WebApp.Client.Components.Editor.CommandHandlers;

public class LoadModelCommandHandler(
    IApiAlphaClient apiAlphaClient,
    HistoryManager historyManager,
    AddEntityContractToEditorCommandHandler addEntityContractToEditorCommandHandler,
    AddEntityContractToCacheCommandHandler addEntityContractToCacheCommandHandler,
    ChangeComponentStateCommandHandler<EditorComponentState> changeComponentStateCommandHandler
) : CommandHandlerBase<LoadModelCommand>(historyManager)
{
    protected override async Task<Result> ExecuteCommandAsync(
        LoadModelCommand command,
        CancellationToken ct = default
    )
    {
        ModelResponseHydrated response = await apiAlphaClient.GetModelHydratedAsync(
            command.ModelId,
            PreconfiguredUnits.N_M,
            ct
        );

        await changeComponentStateCommandHandler.ExecuteAsync(
            new(command.CanvasId, state => state with { LoadedModelId = command.ModelId, }),
            CancellationToken.None
        );

        foreach (var node in response.Nodes)
        {
            await addEntityContractToCacheCommandHandler.ExecuteAsync(
                new(command.ModelId, node),
                CancellationToken.None
            );
            await addEntityContractToEditorCommandHandler.ExecuteAsync(
                new(command.CanvasId, node),
                CancellationToken.None
            );
        }

        foreach (var el in response.Element1Ds)
        {
            await addEntityContractToCacheCommandHandler.ExecuteAsync(
                new(command.ModelId, el),
                CancellationToken.None
            );
            await addEntityContractToEditorCommandHandler.ExecuteAsync(
                new(command.CanvasId, el),
                CancellationToken.None
            );
        }

        foreach (var el in response.PointLoads)
        {
            await addEntityContractToCacheCommandHandler.ExecuteAsync(
                new(command.ModelId, el),
                CancellationToken.None
            );
            await addEntityContractToEditorCommandHandler.ExecuteAsync(
                new(command.CanvasId, el),
                CancellationToken.None
            );
        }

        return Result.Success();
    }
}
