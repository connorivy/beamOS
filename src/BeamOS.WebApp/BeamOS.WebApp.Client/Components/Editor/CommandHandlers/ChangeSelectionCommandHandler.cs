using BeamOs.Common.Api;
using BeamOs.WebApp.Client.Actions.EditorActions;
using BeamOS.WebApp.Client.Components.Editor.Flux.Events;
using BeamOS.WebApp.Client.Repositories;
using BeamOS.WebApp.Client.State;

namespace BeamOS.WebApp.Client.Components.Editor.CommandHandlers;

public class ChangeSelectionCommandHandler(
    EditorApiRepository editorApiRepository,
    HistoryManager historyManager
) : CommandHandlerBase<ChangeSelectionAction>(historyManager)
{
    protected override Task<Result> ExecuteCommandAsync(
        ChangeSelectionAction command,
        CancellationToken ct = default
    ) { }
}
