using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.Common;

namespace BeamOs.StructuralAnalysis.Application.Common;

internal abstract class PutCommandHandlerBase<TId, TEntity, TPutCommand, TResponse>(
    IModelResourceRepositoryIn<TId, TEntity> repository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<TPutCommand, TResponse>
    where TId : struct, IIntBasedId
    where TEntity : BeamOsModelEntity<TId>
{
    public async Task<Result<TResponse>> ExecuteAsync(
        TPutCommand command,
        CancellationToken ct = default
    )
    {
        TEntity entity = this.ToDomainObject(command);
        await repository.Put(entity);
        await unitOfWork.SaveChangesAsync(ct);

        return this.ToResponse(entity);
    }

    protected abstract TEntity ToDomainObject(TPutCommand putCommand);
    protected abstract TResponse ToResponse(TEntity entity);
}

internal abstract class CreateCommandHandlerBase<TId, TEntity, TPutCommand, TResponse>(
    IModelResourceRepository<TId, TEntity> repository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<TPutCommand, TResponse>
    where TId : struct, IIntBasedId
    where TEntity : BeamOsModelEntity<TId>
{
    public async Task<Result<TResponse>> ExecuteAsync(
        TPutCommand command,
        CancellationToken ct = default
    )
    {
        TEntity entity = this.ToDomainObject(command);
        repository.Add(entity);
        await unitOfWork.SaveChangesAsync(ct);

        return this.ToResponse(entity);
    }

    protected abstract TEntity ToDomainObject(TPutCommand putCommand);
    protected abstract TResponse ToResponse(TEntity entity);
}
