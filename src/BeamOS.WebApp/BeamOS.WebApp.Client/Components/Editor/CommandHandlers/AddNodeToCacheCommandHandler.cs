using BeamOs.Common.Api;
using BeamOS.WebApp.Client.Caches;
using BeamOS.WebApp.Client.Components.Editor.Commands;
using BeamOS.WebApp.Client.State;

namespace BeamOS.WebApp.Client.Components.Editor.CommandHandlers;

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
