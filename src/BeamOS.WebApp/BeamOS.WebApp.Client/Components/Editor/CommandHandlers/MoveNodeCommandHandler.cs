using BeamOs.ApiClient;
using BeamOs.Common.Api;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOS.WebApp.Client.Caches;
using BeamOs.WebApp.Client.EditorCommands;
using BeamOs.WebApp.Client.Events.Interfaces;
using BeamOS.WebApp.Client.Repositories;
using BeamOS.WebApp.Client.State;

namespace BeamOS.WebApp.Client.Components.Editor.CommandHandlers;

public class MoveNodeCommandHandler(
    HistoryManager historyManager,
    AllStructuralAnalysisModelCaches allStructuralAnalysisModelCaches,
    IStateRepository<EditorComponentState> editorComponentStateRepository,
    IApiAlphaClient apiAlphaClient
) : VisibleStateCommandHandlerBase<MoveNodeCommand>(historyManager)
{
    protected override async Task<Result> ExecuteCommandAsync(
        MoveNodeCommand command,
        CancellationToken ct = default
    )
    {
        var editorComponentState = editorComponentStateRepository.GetComponentStateByCanvasId(
            command.CanvasId
        );
        var modelId = editorComponentState.LoadedModelId;

        var structuralAnalysisModelCache = allStructuralAnalysisModelCaches.GetOrCreateByModelId(
            modelId.ToString()
        );

        if (command.Source != ClientActionSource.Editor)
        {
            await editorComponentState.EditorApi.ReduceMoveNodeCommandAsync(command);
        }

        var nodeResponse = await apiAlphaClient.PatchNodeAsync(
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
        structuralAnalysisModelCache.AddOrReplace(nodeResponse);

        return Result.Success();
    }
}
