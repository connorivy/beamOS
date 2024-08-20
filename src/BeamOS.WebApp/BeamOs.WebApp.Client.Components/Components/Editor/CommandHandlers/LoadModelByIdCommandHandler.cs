using BeamOs.ApiClient;
using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.Common.Api;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOs.WebApp.Client.Components.Components.Editor.Commands;
using BeamOs.WebApp.Client.Components.Repositories;
using BeamOs.WebApp.Client.Components.State;

namespace BeamOs.WebApp.Client.Components.Components.Editor.CommandHandlers;

public class LoadModelByIdCommandHandler(
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

public class LoadModelCommandHandler(
    IStateRepository<EditorComponentState> editorComponentStateRepository,
    ChangeComponentStateCommandHandler<EditorComponentState> changeComponentStateCommandHandler,
    AddNodesToEditorCommandHandler addNodesToEditorCommandHandler,
    AddElement1dsToEditorCommandHandler addElement1dsToEditorCommandHandler,
    AddPointLoadsToEditorCommandHandler addPointLoadsToEditorCommandHandler,
    HistoryManager historyManager
) : CommandHandlerBase<AddModelToEditorCommand>(historyManager)
{
    protected override async Task<Result> ExecuteCommandAsync(
        AddModelToEditorCommand command,
        CancellationToken ct = default
    )
    {
        var editorComponentState =
            editorComponentStateRepository.GetComponentStateByCanvasId(command.CanvasId)
            ?? throw new Exception(
                $"Could not find editor component corrosponding to canvasId, {command.CanvasId}"
            );

        IEditorApiAlpha? editorApi =
            editorComponentState.EditorApi
            ?? throw new Exception(
                $"Editor api does not exist for canvas with id {command.CanvasId}"
            );

        await editorApi.ClearAsync();
        await changeComponentStateCommandHandler.ExecuteAsync(
            new(
                command.CanvasId,
                state => state with { LoadedModelId = command.ModelResponse.Id, }
            ),
            CancellationToken.None
        );
        await editorApi.SetSettingsAsync(command.ModelResponse.Settings);

        await addNodesToEditorCommandHandler.ExecuteAsync(
            new AddEntitiesToEditorCommand<NodeResponse>(
                command.CanvasId,
                command.ModelResponse.Nodes
            ),
            ct
        );

        await addElement1dsToEditorCommandHandler.ExecuteAsync(
            new AddEntitiesToEditorCommand<Element1DResponse>(
                command.CanvasId,
                command.ModelResponse.Element1ds
            ),
            ct
        );

        await addPointLoadsToEditorCommandHandler.ExecuteAsync(
            new AddEntitiesToEditorCommand<PointLoadResponse>(
                command.CanvasId,
                command.ModelResponse.PointLoads
            ),
            ct
        );

        return Result.Success();
    }
}
