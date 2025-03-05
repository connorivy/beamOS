using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BeamOs.StructuralAnalysis.Infrastructure.Common;

public class ModelLastModifiedUpdater(TimeProvider timeProvider) : SaveChangesInterceptor
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

        var modifiedModelIds = context
            .ChangeTracker
            .Entries()
            .Where(
                e =>
                    e.State != EntityState.Detached
                    && e.State != EntityState.Unchanged
                    && e.Entity is IBeamOsModelEntity
            )
            .Select(e => ((IBeamOsModelEntity)e.Entity).ModelId)
            .Distinct()
            .ToArray();

        DateTimeOffset lastModified = timeProvider.GetUtcNow();

        foreach (var modelId in modifiedModelIds)
        {
            var model = context.Models.Local.FirstOrDefault(m => m.Id == modelId);

            if (model is null)
            {
                model = new("", "", new(UnitSettings.kN_M), modelId);
                context.Models.Attach(model);
            }

            model.LastModified = DateTime.UtcNow;

            context.Entry(model).Property(m => m.LastModified).IsModified = true;
        }

        return result;
    }
}
