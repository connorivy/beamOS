using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.Common;

namespace BeamOs.StructuralAnalysis.Application.Common;

public abstract class GetModelEntityCommandHandler<TId, TEntity, TResponse>(
    IModelResourceRepository<TId, TEntity> entityRepository
) : ICommandHandler<IModelEntity, TResponse>
    where TEntity : BeamOsModelEntity<TId>
    where TId : struct, IIntBasedId
{
    public async Task<Result<TResponse>> ExecuteAsync(
        IModelEntity command,
        CancellationToken ct = default
    )
    {
        var entity = await entityRepository.GetSingle(
            command.ModelId,
            new TId() { Id = command.Id }
        );

        if (entity is null)
        {
            return BeamOsError.NotFound(description: $"Entity with id {command.Id} not found.");
        }

        return this.MapToResponse(entity);
    }

    protected abstract TResponse MapToResponse(TEntity entity);
}
