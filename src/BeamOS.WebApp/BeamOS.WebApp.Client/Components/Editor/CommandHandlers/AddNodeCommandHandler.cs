using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.Common.Api;
using BeamOS.WebApp.Client.Components.Editor.Commands;
using BeamOS.WebApp.Client.Repositories;
using BeamOS.WebApp.Client.State;

namespace BeamOS.WebApp.Client.Components.Editor.CommandHandlers;

public class AddNodeCommandHandler(
    EditorApiRepository editorApiRepository,
    HistoryManager historyManager
) : CommandHandlerBase<AddNodeToEditorCommand>(historyManager)
{
    protected override async Task<Result> ExecuteCommandAsync(
        AddNodeToEditorCommand command,
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
) : CommandHandlerBase<AddElement1dToEditorCommand>(historyManager)
{
    protected override async Task<Result> ExecuteCommandAsync(
        AddElement1dToEditorCommand command,
        CancellationToken ct = default
    )
    {
        IEditorApiAlpha editorApi = editorApiRepository.GetEditorApiByCanvasId(command.CanvasId);
        await editorApi.CreateElement1dAsync(command.Element1d, CancellationToken.None);

        return Result.Success();
    }
}
