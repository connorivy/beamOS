using BeamOs.StructuralAnalysis.Application.Common;

namespace BeamOs.StructuralAnalysis.Infrastructure;

internal sealed class UnitOfWork(StructuralAnalysisDbContext dbContext)
    : IStructuralAnalysisUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return dbContext.SaveChangesAsync(ct);
    }
}
