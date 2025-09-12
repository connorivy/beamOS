using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Application.Common;

internal abstract class BatchPutCommandHandler<TId, TEntity, TBatchPutCommand, TPutRequest>(
    IModelResourceRepositoryIn<TId, TEntity> repository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<TBatchPutCommand, BatchResponse>
    where TId : struct, IIntBasedId
    where TEntity : BeamOsModelEntity<TId>
    where TBatchPutCommand : IModelResourceRequest<TPutRequest[]>
    where TPutRequest : IHasIntId
{
    public async Task<Result<BatchResponse>> ExecuteAsync(
        TBatchPutCommand command,
        CancellationToken ct = default
    )
    {
        HashSet<int> existingIds = (await repository.GetIdsInModel(command.ModelId, ct))
            .Select(id => id.Id)
            .ToHashSet();

        int created = 0;
        int updated = 0;

        EntityStatus[] statuses = new EntityStatus[command.Body.Length];

        for (int i = 0; i < command.Body.Length; i++)
        {
            var putRequest = command.Body[i];
            TEntity entity = this.ToDomainObject(command.ModelId, putRequest);

            if (existingIds.Contains(putRequest.Id))
            {
                await repository.Put(entity);
                updated++;
                statuses[i] = new(putRequest.Id, EntityOperationStatus.Updated);
            }
            else
            {
                repository.Add(entity);
                created++;
                statuses[i] = new(putRequest.Id, EntityOperationStatus.Created);
            }
        }

        await unitOfWork.SaveChangesAsync(ct);

        return new BatchResponse()
        {
            Created = created,
            Updated = updated,
            Deleted = 0,
            Errors = 0,
            EntityStatuses = statuses,
        };
    }

    protected abstract TEntity ToDomainObject(ModelId modelId, TPutRequest putRequest);
}
