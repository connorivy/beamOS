using BeamOs.Application.Common.Interfaces;

namespace BeamOs.Infrastructure;

internal sealed class UnitOfWork(BeamOsStructuralDbContext dbContext) : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return dbContext.SaveChangesAsync(ct);
    }
}
