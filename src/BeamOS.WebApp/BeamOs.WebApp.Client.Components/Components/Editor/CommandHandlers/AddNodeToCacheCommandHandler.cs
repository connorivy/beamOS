using BeamOs.Common.Api;
using BeamOs.WebApp.Client.Components.Caches;
using BeamOs.WebApp.Client.Components.Components.Editor.Commands;
using BeamOs.WebApp.Client.Components.State;

namespace BeamOs.WebApp.Client.Components.Components.Editor.CommandHandlers;

public sealed class AddEntityContractToCacheCommandHandler(
    HistoryManager historyManager,
    AllStructuralAnalysisModelCaches allStructuralAnalysisModelCaches
) : CommandHandlerBase<AddEntityContractToCacheCommand>(historyManager)
{
    protected override Task<Result> ExecuteCommandAsync(
        AddEntityContractToCacheCommand command,
        CancellationToken ct = default
    )
    {
        allStructuralAnalysisModelCaches
            .GetOrCreateByModelId(command.ModelId)
            .AddOrReplace(command.Entity);

        return Task.FromResult(Result.Success());
    }
}
