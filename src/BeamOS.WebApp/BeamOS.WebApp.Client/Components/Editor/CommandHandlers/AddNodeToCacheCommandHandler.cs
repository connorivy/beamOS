using BeamOs.Common.Api;
using BeamOS.WebApp.Client.Caches;
using BeamOS.WebApp.Client.Components.Editor.Commands;
using BeamOS.WebApp.Client.State;

namespace BeamOS.WebApp.Client.Components.Editor.CommandHandlers;

public sealed class AddNodeToCacheCommandHandler(HistoryManager historyManager, AllStructuralAnalysisModelCaches allStructuralAnalysisModelCaches) : CommandHandlerBase<AddNodeToCacheCommand>(historyManager)
{
    protected override Task<Result> ExecuteCommandAsync(AddNodeToCacheCommand command, CancellationToken ct = default)
    {
        var cache = allStructuralAnalysisModelCaches.GetOrCreateByModelId(command.ModelId);
        cache.NodeIdToNodeResponseDict[command.Node.Id] = command.Node;

        return Task.FromResult(Result.Success());
    }
}

public sealed class AddElement1dToCacheCommandHandler(HistoryManager historyManager, AllStructuralAnalysisModelCaches allStructuralAnalysisModelCaches) : CommandHandlerBase<AddElement1dToCacheCommand>(historyManager)
{
    protected override Task<Result> ExecuteCommandAsync(AddElement1dToCacheCommand command, CancellationToken ct = default)
    {
        var cache = allStructuralAnalysisModelCaches.GetOrCreateByModelId(command.ModelId);
        cache.Element1dIdToElement1dResponseDict[command.Element1d.Id] = command.Element1d;

        return Task.FromResult(Result.Success());
    }
}
