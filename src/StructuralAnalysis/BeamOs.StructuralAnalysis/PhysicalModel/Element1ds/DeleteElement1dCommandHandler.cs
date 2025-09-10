using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;

public class DeleteElement1dCommandHandler(
    IElement1dRepository element1dRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<IModelEntity, ModelEntityResponse>
{
    public async Task<Result<ModelEntityResponse>> ExecuteAsync(
        IModelEntity command,
        CancellationToken ct = default
    )
    {
        await element1dRepository.RemoveById(command.ModelId, command.Id, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new ModelEntityResponse(command.Id, command.ModelId);
    }
}
