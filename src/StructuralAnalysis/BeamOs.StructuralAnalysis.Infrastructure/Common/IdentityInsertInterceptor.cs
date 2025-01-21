using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.Common;
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

        DbContextOptions<StructuralAnalysisDbContext> options = new();
        if (entriesWithIds.Any())
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

    //public override async ValueTask<int> SavingChangesAsync(
    //    DbContextEventData eventData,
    //    InterceptionResult<int> result,
    //    CancellationToken cancellationToken = default
    //)
    //{
    //    var context = eventData.Context;
    //    if (context == null)
    //        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    //    var entriesWithIds = context
    //        .ChangeTracker
    //        .Entries()
    //        .Where(
    //            e =>
    //                e.State == EntityState.Added
    //                && e.CurrentValues.PropertyNames.Contains("Id")
    //                && (int)e.CurrentValues["Id"] > 0
    //        )
    //        .ToList();
    //    if (entriesWithIds.Any())
    //    {
    //        await context
    //            .Database
    //            .ExecuteSqlRawAsync("SET IDENTITY_INSERT MyEntities ON", cancellationToken);
    //    }
    //    var saveChangesResult = await base.SavingChangesAsync(eventData, result, cancellationToken);
    //    if (entriesWithIds.Any())
    //    {
    //        await context
    //            .Database
    //            .ExecuteSqlRawAsync("SET IDENTITY_INSERT MyEntities OFF", cancellationToken);
    //    }
    //    return saveChangesResult;
    //}
}
