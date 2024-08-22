using BeamOs.Common.Api;
using BeamOs.WebApp.Client.Components.Caches;
using BeamOs.WebApp.Client.Components.Components.Editor.Commands;

namespace BeamOs.WebApp.Client.Components.Components.Editor.CommandHandlers;

public sealed class AddEntityContractToCacheCommandHandler(
    AllStructuralAnalysisModelCaches allStructuralAnalysisModelCaches
) : CommandHandlerBase<AddEntityContractToCacheCommand>
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
