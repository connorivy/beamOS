using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

namespace BeamOs.StructuralAnalysis.Application.SystemOperations;

internal sealed class ModelRestoreCommandHandler(GetModelCommandHandler getModelCommandHandler)
    : ICommandHandler<ModelResourceRequest<DateTimeOffset>, ModelResponse>
{
    public async Task<Result<ModelResponse>> ExecuteAsync(
        ModelResourceRequest<DateTimeOffset> command,
        CancellationToken ct = default
    ) => await getModelCommandHandler.ExecuteAsync(command.ModelId, ct);
}
