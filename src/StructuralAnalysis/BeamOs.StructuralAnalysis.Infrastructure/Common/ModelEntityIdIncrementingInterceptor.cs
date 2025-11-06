using System.Runtime.InteropServices;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BeamOs.StructuralAnalysis.Infrastructure.Common;

internal class ModelEntityIdIncrementingInterceptor(TimeProvider timeProvider)
    : SaveChangesInterceptor
{
    private static readonly SemaphoreSlim semaphore = new(1, 1);

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
#if Sqlite
            // Sqlite does not support composite keys with auto-increment
            .Where(e => e.Entity is not ModelProposal and not DeleteModelEntityProposal)
#endif
            .Select(e => (IBeamOsModelEntity)e.Entity)
            .ToArray();

        if (addedModelEntities.Length == 0)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        await semaphore.WaitAsync(cancellationToken);
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

            var entitiesWithDefaultIds = entityInfoGroup
                .Where(info => info.Id == 0)
                .GroupBy(info => info.Type)
                .ToArray();

            var currentModel =
                await context
                    .Models.Where(m => m.Id == entityInfoGroup.Key)
                    .FirstOrDefaultAsync(cancellationToken)
                ?? throw new InvalidOperationException(
                    $"Could not find model with id {entityInfoGroup.Key} when trying to assign ids to new entities."
                );

            // idResults ??= new
            // {
            //     MaxNodeId = 0,
            //     MaxElement1dId = 0,
            //     MaxMaterialId = 0,
            //     MaxSectionProfileId = 0,
            //     MaxPointLoadId = 0,
            //     MaxMomentLoadId = 0,
            //     MaxResultSetId = 0,
            //     MaxLoadCaseId = 0,
            //     MaxLoadCombinationId = 0,
            // };

            Dictionary<Type, int> entityTypeToMaxIdDict = new()
            {
                { typeof(Node), currentModel.MaxNodeId },
                { typeof(InternalNode), currentModel.MaxInternalNodeId },
                { typeof(Element1d), currentModel.MaxElement1dId },
                { typeof(Material), currentModel.MaxMaterialId },
                { typeof(SectionProfile), currentModel.MaxSectionProfileId },
                { typeof(SectionProfileFromLibrary), currentModel.MaxSectionProfileFromLibraryId },
                { typeof(PointLoad), currentModel.MaxPointLoadId },
                { typeof(MomentLoad), currentModel.MaxMomentLoadId },
                { typeof(LoadCase), currentModel.MaxLoadCaseId },
                { typeof(LoadCombination), currentModel.MaxLoadCombinationId },
                { typeof(ModelProposal), currentModel.MaxModelProposalId },
            };

            foreach (var entityInfoByType in entitiesWithDefaultIds)
            {
                var entityType = entityInfoByType.Key;
                if (!entityTypeToMaxIdDict.ContainsKey(entityType))
                {
                    throw new InvalidOperationException(
                        $"Could not find next id for entity of type {entityType}"
                    );
                }

                ref int maxId = ref CollectionsMarshal.GetValueRefOrNullRef(
                    entityTypeToMaxIdDict,
                    entityType
                );

                HashSet<int>? takenIds = entityTypeToTakenIdsDict.GetValueOrDefault(entityType);
                foreach (var entityInfo in entityInfoByType)
                {
                    do
                    {
                        maxId++;
                    } while (takenIds?.Contains(maxId) ?? false);
                    entityInfo.Entity.SetIntId(maxId);
                }
            }

            currentModel.MaxNodeId = entityTypeToMaxIdDict[typeof(Node)];
            currentModel.MaxInternalNodeId = entityTypeToMaxIdDict[typeof(InternalNode)];
            currentModel.MaxElement1dId = entityTypeToMaxIdDict[typeof(Element1d)];
            currentModel.MaxMaterialId = entityTypeToMaxIdDict[typeof(Material)];
            currentModel.MaxSectionProfileId = entityTypeToMaxIdDict[typeof(SectionProfile)];
            currentModel.MaxSectionProfileFromLibraryId = entityTypeToMaxIdDict[
                typeof(SectionProfileFromLibrary)
            ];
            currentModel.MaxPointLoadId = entityTypeToMaxIdDict[typeof(PointLoad)];
            currentModel.MaxMomentLoadId = entityTypeToMaxIdDict[typeof(MomentLoad)];
            currentModel.MaxLoadCaseId = entityTypeToMaxIdDict[typeof(LoadCase)];
            currentModel.MaxLoadCombinationId = entityTypeToMaxIdDict[typeof(LoadCombination)];
            currentModel.MaxModelProposalId = entityTypeToMaxIdDict[typeof(ModelProposal)];
            currentModel.LastModified = timeProvider.GetUtcNow();
        }

        semaphore.Release();
        return result;
    }
}
