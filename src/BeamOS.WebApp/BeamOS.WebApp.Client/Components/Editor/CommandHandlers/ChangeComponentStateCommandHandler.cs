using BeamOs.Common.Api;
using BeamOS.WebApp.Client.Components.Editor.Commands;
using BeamOS.WebApp.Client.Repositories;
using BeamOS.WebApp.Client.State;

namespace BeamOS.WebApp.Client.Components.Editor.CommandHandlers;

public class ChangeComponentStateCommandHandler(
    IStateRepository<EditorComponentState> editorComponentStateRepository,
    HistoryManager historyManager
) : VisibleStateCommandHandlerBase<ChangeComponentStateCommand>(historyManager)
{
    protected override Task<Result> ExecuteCommandAsync(ChangeComponentStateCommand command, CancellationToken ct = default)
    {
        EditorComponentState? state = editorComponentStateRepository.GetComponentStateByCanvasId(command.CanvasId);

        if (state == null)
        {
            return Task.FromResult(Result.Failure(BeamOsError.Todo));
        }

        editorComponentStateRepository.SetComponentStateForCanvasId(command.CanvasId, command.NewEditorComponentState);

        return Task.FromResult(Result.Success());
    }
}

public class ChangeComponentStateCommandHandler<TState>(
    IStateRepository<TState> stateRepository,
    HistoryManager historyManager
) : VisibleStateCommandHandlerBase<ChangeComponentStateCommand<TState>>(historyManager)
{
    protected override Task<Result> ExecuteCommandAsync(ChangeComponentStateCommand<TState> command, CancellationToken ct = default)
    {
        TState? state = stateRepository.GetComponentStateByCanvasId(command.CanvasId);

        if (state == null)
        {
            return Task.FromResult(Result.Failure(BeamOsError.Todo));
        }

        stateRepository.SetComponentStateForCanvasId(command.CanvasId, command.StateMutation(state));

        return Task.FromResult(Result.Success());
    }
}
