using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

internal sealed class DeleteModelCommandHandler(
    IModelRepository modelRepository,
    IStructuralAnalysisUnitOfWork uow
) : ICommandHandler<Guid, bool>
{
    public async Task<Result<bool>> ExecuteAsync(Guid command, CancellationToken ct = default)
    {
        var model = await modelRepository.GetSingle(command, ct);
        if (model is null)
        {
            return BeamOsError.NotFound(description: $"Model with id {command} not found");
        }
        modelRepository.Remove(model);
        await uow.SaveChangesAsync(ct);

        return true;
    }
}
