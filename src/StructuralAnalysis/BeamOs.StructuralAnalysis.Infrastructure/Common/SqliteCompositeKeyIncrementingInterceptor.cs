using System.Runtime.InteropServices;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BeamOs.StructuralAnalysis.Infrastructure.Common;

internal class SqliteCompositeKeyIncrementingInterceptor : SaveChangesInterceptor
{
    private readonly Dictionary<ModelId, Dictionary<Type, int>> modelIdToTypeToMaxIdDict = [];
    private readonly Dictionary<
        ModelId,
        Dictionary<ModelProposalId, Dictionary<Type, int>>
    > modelProposalIdToTypeToMaxIdDict = [];

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context is not StructuralAnalysisDbContext context)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var addedModelEntities = context
            .ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added && e.Entity is IBeamOsModelEntity)
            .Where(e => e.Entity is ModelProposal or DeleteModelEntityProposal)
            .Select(e => (IBeamOsModelEntity)e.Entity)
            .ToArray();

        var addedModelProposalEntities = context
            .ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added && e.Entity is IBeamOsModelProposalEntity)
            .Select(e => (IBeamOsModelProposalEntity)e.Entity)
            .ToArray();

        if (addedModelEntities.Length == 0)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        foreach (
            var entityInfoGroup in addedModelEntities
                .Select(e => new
                {
                    Entity = e,
                    Id = e.GetIntId(),
                    Type = e.GetType(),
                })
                .GroupBy(e => e.Entity.ModelId)
        )
        {
            var entityTypeToTakenIdsDict = entityInfoGroup
                .GroupBy(i => i.Type)
                .ToDictionary(info => info.Key, info => info.Select(i => i.Id).ToHashSet());

            var entityInfoByType2 = entityInfoGroup
                .Where(info => info.Id == 0)
                .GroupBy(info => info.Type)
                .ToArray();

            if (
                !this.modelIdToTypeToMaxIdDict.TryGetValue(
                    entityInfoGroup.Key,
                    out var additionalInMemoryStorage
                )
            )
            {
                additionalInMemoryStorage = new()
                {
                    { typeof(ModelProposal), 0 },
                    { typeof(DeleteModelEntityProposal), 0 },
                };
                this.modelIdToTypeToMaxIdDict[entityInfoGroup.Key] = additionalInMemoryStorage;
            }

            foreach (var entityInfoByType in entityInfoByType2)
            {
                var entityType = entityInfoByType.Key;
                if (!additionalInMemoryStorage.ContainsKey(entityType))
                {
                    throw new InvalidOperationException(
                        $"Could not find next id for entity of type {entityType}"
                    );
                }

                ref int maxId = ref CollectionsMarshal.GetValueRefOrNullRef(
                    additionalInMemoryStorage,
                    entityType
                );

                HashSet<int>? takenIds = entityTypeToTakenIdsDict.GetValueOrDefault(entityType);
                foreach (var entityInfo in entityInfoByType)
                {
                    while (takenIds?.Contains(++maxId) ?? false)
                    {
                        // do nothing
                    }
                    entityInfo.Entity.SetIntId(maxId);
                }
            }
        }

        foreach (
            var entityInfoGroup in addedModelProposalEntities
                .Select(e => new
                {
                    Entity = e,
                    Id = e.GetIntId(),
                    Type = e.GetType(),
                })
                .GroupBy(e => e.Entity.ModelId)
        )
        {
            if (
                !this.modelProposalIdToTypeToMaxIdDict.TryGetValue(
                    entityInfoGroup.Key,
                    out var modelProposalStorageByModel
                )
            )
            {
                modelProposalStorageByModel = [];
                this.modelProposalIdToTypeToMaxIdDict[entityInfoGroup.Key] =
                    modelProposalStorageByModel;
            }
            foreach (
                var entityInfoGroupByProposal in entityInfoGroup.GroupBy(e =>
                    e.Entity.ModelProposalId
                )
            )
            {
                if (
                    !modelProposalStorageByModel.TryGetValue(
                        entityInfoGroupByProposal.Key,
                        out var modelProposalEntityIdStorage
                    )
                )
                {
                    modelProposalEntityIdStorage = new()
                    {
                        { typeof(NodeProposal), 0 },
                        { typeof(InternalNodeProposal), 0 },
                        { typeof(Element1dProposal), 0 },
                        { typeof(MaterialProposal), 0 },
                        { typeof(SectionProfileProposal), 0 },
                        { typeof(SectionProfileProposalFromLibrary), 0 },
                        { typeof(ProposalIssue), 0 },
                        // { typeof(DeleteModelEntityProposal), 0 },
                    };
                    modelProposalStorageByModel[entityInfoGroupByProposal.Key] =
                        modelProposalEntityIdStorage;
                }

                var entityTypeToTakenIdsDict = entityInfoGroupByProposal
                    .GroupBy(i => i.Type)
                    .ToDictionary(info => info.Key, info => info.Select(i => i.Id).ToHashSet());

                var entityInfoByType2 = entityInfoGroupByProposal
                    .Where(info => info.Id == 0)
                    .GroupBy(info => info.Type)
                    .ToArray();

                foreach (var entityInfoByType in entityInfoByType2)
                {
                    var entityType = entityInfoByType.Key;
                    if (!modelProposalEntityIdStorage.ContainsKey(entityType))
                    {
                        throw new InvalidOperationException(
                            $"Could not find next id for entity of type {entityType}"
                        );
                    }

                    ref int maxId = ref CollectionsMarshal.GetValueRefOrNullRef(
                        modelProposalEntityIdStorage,
                        entityType
                    );

                    HashSet<int>? takenIds = entityTypeToTakenIdsDict.GetValueOrDefault(entityType);
                    foreach (var entityInfo in entityInfoByType)
                    {
                        while (takenIds?.Contains(++maxId) ?? false)
                        {
                            // do nothing
                        }
                        entityInfo.Entity.SetIntId(maxId);
                    }
                }
            }
        }

        return result;
    }
}
