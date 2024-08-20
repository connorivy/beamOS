using BeamOs.Common.Api;
using BeamOs.WebApp.Client.Components.Repositories;
using BeamOs.WebApp.Client.Components.State;
using BeamOs.WebApp.Client.EditorCommands;

namespace BeamOs.WebApp.Client.Components.Components.Editor.CommandHandlers;

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
