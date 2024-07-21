using BeamOs.Common.Api;
using BeamOs.WebApp.Client.EditorCommands;
using BeamOS.WebApp.Client.Repositories;
using BeamOS.WebApp.Client.State;

namespace BeamOS.WebApp.Client.Components.Editor.CommandHandlers;

public class ChangeSelectionCommandHandler(
    IStateRepository<EditorComponentState> editorComponentStateRepository,
    HistoryManager historyManager
) : VisibleStateCommandHandlerBase<ChangeSelectionCommand>(historyManager)
{
    protected override Task<Result> ExecuteCommandAsync(
        ChangeSelectionCommand command,
        CancellationToken ct = default
    )
    {
        EditorComponentState? state = editorComponentStateRepository.GetComponentStateByCanvasId(
            command.CanvasId
        );

        if (state == null)
        {
            return Task.FromResult(Result.Failure(BeamOsError.Todo));
        }

        editorComponentStateRepository.SetComponentStateForCanvasId(
            command.CanvasId,
            state with
            {
                SelectedObjects = command.SelectedObjects
            }
        );

        return Task.FromResult(Result.Success());
    }
}
