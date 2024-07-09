using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.Common.Api;
using BeamOS.WebApp.Client.Components.Editor.Flux.Events;
using BeamOS.WebApp.Client.Repositories;
using BeamOS.WebApp.Client.State;

namespace BeamOS.WebApp.Client.Components.Editor.CommandHandlers;

public class AddNodeCommandHandler(
    EditorApiRepository editorApiRepository,
    HistoryManager historyManager
) : CommandHandlerBase<AddNodeAction>(historyManager)
{
    protected override async Task<Result> ExecuteCommandAsync(
        AddNodeAction command,
        CancellationToken ct = default
    )
    {
        IEditorApiAlpha editorApi = editorApiRepository.GetEditorApiByCanvasId(command.CanvasId);
        await editorApi.CreateNodeAsync(command.Node, CancellationToken.None);

        return Result.Success();
    }
}

public class AddElement1dCommandHandler(
    EditorApiRepository editorApiRepository,
    HistoryManager historyManager
) : CommandHandlerBase<AddElement1dAction>(historyManager)
{
    protected override async Task<Result> ExecuteCommandAsync(
        AddElement1dAction command,
        CancellationToken ct = default
    )
    {
        IEditorApiAlpha editorApi = editorApiRepository.GetEditorApiByCanvasId(command.CanvasId);
        await editorApi.CreateElement1dAsync(command.Element1d, CancellationToken.None);

        return Result.Success();
    }
}
