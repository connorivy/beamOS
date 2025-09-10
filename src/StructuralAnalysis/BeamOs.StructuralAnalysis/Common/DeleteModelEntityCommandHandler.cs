using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Domain.Common;

namespace BeamOs.StructuralAnalysis.Application.Common;

public abstract class DeleteModelEntityCommandHandler<TId, TEntity>(
    IModelResourceRepositoryIn<TId, TEntity> entityRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<IModelEntity, ModelEntityResponse>
    where TEntity : BeamOsModelEntity<TId>
    where TId : struct, IIntBasedId
{
    public async Task<Result<ModelEntityResponse>> ExecuteAsync(
        IModelEntity command,
        CancellationToken ct = default
    )
    {
        await entityRepository.RemoveById(command.ModelId, new TId() { Id = command.Id });
        await unitOfWork.SaveChangesAsync(ct);

        return new ModelEntityResponse(command.Id, command.ModelId);
    }
}
