using System.Diagnostics;
using System.Runtime.InteropServices;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BeamOs.StructuralAnalysis.Infrastructure.Common;

public class ModelEntityIdIncrementingInterceptor : SaveChangesInterceptor
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
            .Where(e => e.State == EntityState.Added && e.Entity is IBeamOsModelEntity)
            .Select(e => (IBeamOsModelEntity)e.Entity)
            .ToList();

        if (addedModelEntities.Count == 0)
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
            var idResults = await context
                .Models.Where(m => m.Id == entityInfoGroup.Key)
                .Select(m => new
                {
                    MaxNodeId = m.Nodes.Max(el => (int?)el.Id) ?? 0,
                    MaxElement1dId = m.Element1ds.Max(el => (int?)el.Id) ?? 0,
                    MaxMaterialId = m.Materials.Max(el => (int?)el.Id) ?? 0,
                    MaxSectionProfileId = m.SectionProfiles.Max(el => (int?)el.Id) ?? 0,
                    MaxPointLoadId = m.PointLoads.Max(el => (int?)el.Id) ?? 0,
                    MaxMomentLoadId = m.MomentLoads.Max(el => (int?)el.Id) ?? 0,
                    MaxResultSetId = m.ResultSets.Max(el => (int?)el.Id) ?? 0,
                })
                .FirstOrDefaultAsync(cancellationToken);

            idResults ??= new
            {
                MaxNodeId = 0,
                MaxElement1dId = 0,
                MaxMaterialId = 0,
                MaxSectionProfileId = 0,
                MaxPointLoadId = 0,
                MaxMomentLoadId = 0,
                MaxResultSetId = 0,
            };

            Dictionary<Type, int> entityTypeToMaxIdDict = new()
            {
                { typeof(Node), idResults.MaxNodeId },
                { typeof(Element1d), idResults.MaxElement1dId },
                { typeof(Material), idResults.MaxMaterialId },
                { typeof(SectionProfile), idResults.MaxSectionProfileId },
                { typeof(PointLoad), idResults.MaxPointLoadId },
                { typeof(MomentLoad), idResults.MaxMomentLoadId },
                { typeof(ResultSet), idResults.MaxResultSetId },
            };

            Dictionary<Type, HashSet<int>> entityTypeToTakenIdsDict = entityInfoGroup
                .GroupBy(i => i.Type)
                .ToDictionary(info => info.Key, info => info.Select(i => i.Id).ToHashSet());

            foreach (
                var entityInfoByType in entityInfoGroup
                    .Where(info => info.Id == 0)
                    .GroupBy(info => info.Type)
            )
            {
                var entityType = entityInfoByType.Key;
                Debug.Assert(entityTypeToMaxIdDict.ContainsKey(entityType));

                ref int maxId = ref CollectionsMarshal.GetValueRefOrNullRef(
                    entityTypeToMaxIdDict,
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

        return result;
    }
}
