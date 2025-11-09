using System.Runtime.InteropServices;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BeamOs.StructuralAnalysis.Infrastructure.Common;

internal class ModelProposalEntityIdIncrementingInterceptor(TimeProvider timeProvider)
    : SaveChangesInterceptor
{
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
            .Where(e => e.State == EntityState.Added)
            .OfType<IBeamOsModelProposalEntity>()
            .ToArray();

        if (addedModelEntities.Length == 0)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        foreach (
            var entityInfoGroupByModel in addedModelEntities
                .Select(e => new
                {
                    Entity = e,
                    Id = e.GetIntId(),
                    Type = e.GetType(),
                })
                .GroupBy(e => e.Entity.ModelId)
        )
        {
            foreach (
                var entityInfoGroup in entityInfoGroupByModel.GroupBy(e => e.Entity.ModelProposalId)
            )
            {
                var entityTypeToTakenIdsDict = entityInfoGroup
                    .GroupBy(i => i.Type)
                    .ToDictionary(info => info.Key, info => info.Select(i => i.Id).ToHashSet());

                var entitiesWithDefaultIds = entityInfoGroup
                    .Where(info => info.Id == 0)
                    .GroupBy(info => info.Type)
                    .ToArray();

                var currentModelProposal =
                    await context
                        .ModelProposals.AsSplitQuery()
                        .Where(m =>
                            m.ModelId == entityInfoGroupByModel.Key && m.Id == entityInfoGroup.Key
                        )
                        .FirstOrDefaultAsync(cancellationToken)
                    ?? throw new InvalidOperationException(
                        $"Could not find model proposal with id {entityInfoGroup.Key} when trying to assign ids to new entities."
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

                // NodeProposal and InternalNodeProposal both create NodeDefinition entities that share the same table
                var maxNodeProposalId = Math.Max(currentModelProposal.MaxNodeId, currentModelProposal.MaxInternalNodeId);
                
                Dictionary<Type, int> entityTypeToMaxIdDict = new()
                {
                    { typeof(NodeProposal), maxNodeProposalId },
                    { typeof(InternalNodeProposal), maxNodeProposalId },
                    { typeof(Element1dProposal), currentModelProposal.MaxElement1dId },
                    { typeof(MaterialProposal), currentModelProposal.MaxMaterialId },
                    { typeof(SectionProfileProposal), currentModelProposal.MaxSectionProfileId },
                    // {
                    //     typeof(SectionProfileFromLibrary),
                    //     currentModel.MaxSectionProfileFromLibraryId
                    // },
                    // { typeof(PointLoad), currentModel.MaxPointLoadId },
                    // { typeof(MomentLoad), currentModel.MaxMomentLoadId },
                    // { typeof(LoadCase), currentModel.MaxLoadCaseId },
                    // { typeof(LoadCombination), currentModel.MaxLoadCombinationId },
                    // { typeof(ModelProposal), currentModel.MaxModelProposalId },
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

                // Update both MaxNodeId and MaxInternalNodeId to the same value since they will create entities in the same table
                var finalMaxNodeProposalId = Math.Max(
                    entityTypeToMaxIdDict[typeof(NodeProposal)],
                    entityTypeToMaxIdDict[typeof(InternalNodeProposal)]
                );
                currentModelProposal.MaxNodeId = finalMaxNodeProposalId;
                currentModelProposal.MaxInternalNodeId = finalMaxNodeProposalId;
                currentModelProposal.MaxElement1dId = entityTypeToMaxIdDict[
                    typeof(Element1dProposal)
                ];
                currentModelProposal.MaxMaterialId = entityTypeToMaxIdDict[
                    typeof(MaterialProposal)
                ];
                currentModelProposal.MaxSectionProfileId = entityTypeToMaxIdDict[
                    typeof(SectionProfileProposal)
                ];
                // currentModelProposal.MaxSectionProfileFromLibraryId = entityTypeToMaxIdDict[
                //     typeof(SectionProfileFromLibrary)
                // ];
                // currentModelProposal.MaxPointLoadId = entityTypeToMaxIdDict[typeof(PointLoad)];
                // currentModelProposal.MaxMomentLoadId = entityTypeToMaxIdDict[typeof(MomentLoad)];
                // currentModelProposal.MaxLoadCaseId = entityTypeToMaxIdDict[typeof(LoadCase)];
                // currentModelProposal.MaxLoadCombinationId = entityTypeToMaxIdDict[typeof(LoadCombination)];
                currentModelProposal.LastModified = timeProvider.GetUtcNow();
            }
        }

        return result;
    }
}
