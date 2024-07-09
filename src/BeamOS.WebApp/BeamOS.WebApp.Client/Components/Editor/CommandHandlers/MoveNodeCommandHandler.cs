using BeamOs.ApiClient;
using BeamOs.Common.Api;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.WebApp.Client.Actions.EditorActions;
using BeamOS.WebApp.Client.Caches;
using BeamOs.WebApp.Client.Events.Interfaces;
using BeamOS.WebApp.Client.Repositories;
using BeamOS.WebApp.Client.State;

namespace BeamOS.WebApp.Client.Components.Editor.CommandHandlers;

public class MoveNodeCommandHandler(
    HistoryManager historyManager,
    AllStructuralAnalysisModelCaches allStructuralAnalysisModelCaches,
    ModelIdRepository modelIdRepository,
    EditorApiRepository editorApiRepository,
    IApiAlphaClient apiAlphaClient
) : CommandHandlerBase<MoveNodeAction>(historyManager)
{
    protected override async Task<Result> ExecuteCommandAsync(
        MoveNodeAction command,
        CancellationToken ct = default
    )
    {
        var modelId = modelIdRepository.GetModelIdByCanvasId(command.CanvasId);
        var structuralAnalysisModelCache = allStructuralAnalysisModelCaches.GetOrCreateByModelId(
            modelId.ToString()
        );

        if (command.Source != ClientActionSource.Editor)
        {
            var editorApi = editorApiRepository.GetEditorApiByCanvasId(command.CanvasId);
            await editorApi.ReduceMoveNodeActionAsync(command);
        }

        await apiAlphaClient.PatchNodeAsync(
            new PatchNodeRequest()
            {
                NodeId = command.NodeId.ToString(),
                LocationPoint = new()
                {
                    LengthUnit = "Meter",
                    XCoordinate = command.NewLocation.X,
                    YCoordinate = command.NewLocation.Y,
                    ZCoordinate = command.NewLocation.Z
                }
            },
            command.NodeId.ToString()
        );

        return Result.Success();
    }
}
