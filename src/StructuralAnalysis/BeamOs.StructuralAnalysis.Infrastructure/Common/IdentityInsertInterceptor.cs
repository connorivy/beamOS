using System.Runtime.InteropServices;
using BeamOs.Common.Contracts;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BeamOs.StructuralAnalysis.Infrastructure.Common;

public class IdentityInsertInterceptor(
    DbContextOptions<StructuralAnalysisDbContext> dbContextOptions
) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        var context = eventData.Context;
        if (context == null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var allEntities = context.ChangeTracker.Entries().ToList();

        var entriesWithIds = allEntities
            .Where(
                e =>
                    e.State == EntityState.Added
                    && e.Entity is IBeamOsModelEntity beamOsModelEntity
                    && beamOsModelEntity.GetIntId() != 0
            )
            .ToList();

        //var entriesWithIds = entries.Where(e => (int)e.CurrentValues["Id"] > 0).ToList();
        var entriesWithoutIds = allEntities.Except(entriesWithIds);

        if (entriesWithIds.Count > 0)
        {
            using var contextWithIds = new StructuralAnalysisDbContext(dbContextOptions);

            var entityTypesWithIds = entriesWithIds
                .GroupBy(e => e.Entity.GetType())
                .Select(g => new { EntityType = g.Key, Entries = g.ToList() })
                .ToList();

            foreach (var entityType in entityTypesWithIds)
            {
                using var transaction = await contextWithIds.Database.BeginTransactionAsync();
                var modelEntityType = contextWithIds.Model.FindEntityType(entityType.EntityType);

                await contextWithIds
                    .Database
                    .ExecuteSqlRawAsync(
                        $"SET IDENTITY_INSERT {modelEntityType.GetTableName()} ON",
                        cancellationToken
                    );
                await this.SaveEntitiesAsync(contextWithIds, entityType.Entries);
                await contextWithIds
                    .Database
                    .ExecuteSqlRawAsync(
                        $"SET IDENTITY_INSERT {modelEntityType.GetTableName()} OFF",
                        cancellationToken
                    );
                await transaction.CommitAsync();
            }
        }

        using (var contextWithoutIds = new StructuralAnalysisDbContext(dbContextOptions))
        {
            if (entriesWithoutIds.Any())
            {
                await this.SaveEntitiesAsync(contextWithoutIds, entriesWithoutIds);
            }
        }

        context.ChangeTracker.Clear();

        return result;
    }

    private async Task SaveEntitiesAsync(DbContext context, IEnumerable<EntityEntry> entries)
    {
        foreach (var entry in entries)
        {
            var newEntry = context.Add(entry.Entity);
            newEntry.State = entry.State;
            //context.Entry(entry.Entity).State = EntityState.Added;
        }
        await context.SaveChangesAsync();
    }
}

public class ModelEntityIdIncrementingInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        var context = (StructuralAnalysisDbContext)eventData.Context;
        if (context == null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var addedModelEntities = context
            .ChangeTracker
            .Entries()
            .Where(e => e.State == EntityState.Added && e.Entity is IBeamOsModelEntity)
            .Select(e => (IBeamOsModelEntity)e.Entity)
            .ToList();

        if (addedModelEntities.Count == 0)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        Dictionary<Type, HashSet<int>> entityTypeToTakenIdsDict = [];

        foreach (var entity in addedModelEntities.Where(e => e.GetIntId() != 0))
        {
            int entityId = entity.GetIntId();
            if (!entityTypeToTakenIdsDict.TryGetValue(entity.GetType(), out var takenIds))
            {
                takenIds = new();
                entityTypeToTakenIdsDict.Add(entity.GetType(), takenIds);
            }

            if (!takenIds.Add(entityId))
            {
                throw new InvalidOperationException(
                    $"Cannot add multiple entities of type {entity.GetType()} with Id {entityId}"
                );
            }
        }

        var modelId = addedModelEntities[0].ModelId; // todo: assuming same model id for all entities...

        var idResults = await context
            .Models
            .Where(m => m.Id == modelId)
            .Select(
                m =>
                    new
                    {
                        MaxNodeId = m.Nodes.Max(el => (int?)el.Id) ?? 0,
                        MaxElement1dId = m.Element1ds.Max(el => (int?)el.Id) ?? 0,
                        MaxMaterialId = m.Materials.Max(el => (int?)el.Id) ?? 0,
                        MaxSectionProfileId = m.SectionProfiles.Max(el => (int?)el.Id) ?? 0,
                        MaxPointLoadId = m.PointLoads.Max(el => (int?)el.Id) ?? 0,
                        MaxMomentLoadId = m.MomentLoads.Max(el => (int?)el.Id) ?? 0,
                        MaxResultSetId = m.ResultSets.Max(el => (int?)el.Id) ?? 0,
                    }
            )
            .FirstAsync(cancellationToken);

        Dictionary<Type, int> entityTypeToMaxIdDict =
            new()
            {
                { typeof(Node), idResults.MaxNodeId },
                { typeof(Element1d), idResults.MaxElement1dId },
                { typeof(Material), idResults.MaxMaterialId },
                { typeof(SectionProfile), idResults.MaxSectionProfileId },
                { typeof(PointLoad), idResults.MaxPointLoadId },
                { typeof(MomentLoad), idResults.MaxMomentLoadId },
                { typeof(ResultSet), idResults.MaxResultSetId },
            };

        foreach (var entity in addedModelEntities.Where(e => e.GetIntId() == 0))
        {
            ref int maxId = ref CollectionsMarshal.GetValueRefOrNullRef(
                entityTypeToMaxIdDict,
                entity.GetType()
            );
            entity.SetIntId(++maxId);
        }

        return result;
    }

    private async Task SaveEntitiesAsync(DbContext context, IEnumerable<EntityEntry> entries)
    {
        foreach (var entry in entries)
        {
            var newEntry = context.Add(entry.Entity);
            newEntry.State = entry.State;
            //context.Entry(entry.Entity).State = EntityState.Added;
        }
        await context.SaveChangesAsync();
    }
}
