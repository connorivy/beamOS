using BeamOs.ApiClient;
using BeamOs.Common.Api;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOS.WebApp.Client.Components.Editor.Commands;
using BeamOS.WebApp.Client.Repositories;
using BeamOS.WebApp.Client.State;

namespace BeamOS.WebApp.Client.Components.Editor.CommandHandlers;

public class LoadModelCommandHandler(
    IApiAlphaClient apiAlphaClient,
    HistoryManager historyManager,
    AddNodeCommandHandler addNodeCommandHandler,
    AddElement1dCommandHandler addElement1dCommandHandler,
    ModelIdRepository modelIdRepository,
    AddElement1dToCacheCommandHandler addElement1DToCacheCommandHandler,
    AddNodeToCacheCommandHandler addNodeToCacheCommandHandler,
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

        await changeComponentStateCommandHandler.ExecuteAsync(new(command.CanvasId, state => state with
        {
            LoadedModelId = command.ModelId,
        }), CancellationToken.None);

        modelIdRepository.SetModelIdForCanvasId(command.CanvasId, command.ModelId);

        foreach (var node in response.Nodes)
        {
            await addNodeToCacheCommandHandler.ExecuteAsync(new(command.ModelId, node), CancellationToken.None);
            await addNodeCommandHandler.ExecuteAsync(
                new AddNodeToEditorCommand(command.CanvasId, node),
                CancellationToken.None
            );
        }

        foreach (var el in response.Element1Ds)
        {
            await addElement1DToCacheCommandHandler.ExecuteAsync(new(command.ModelId, el), CancellationToken.None);
            await addElement1dCommandHandler.ExecuteAsync(
                new AddElement1dToEditorCommand(command.CanvasId, el),
                CancellationToken.None
            );
        }

        return Result.Success();
    }
}
