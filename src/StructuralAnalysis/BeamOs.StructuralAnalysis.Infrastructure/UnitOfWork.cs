using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Infrastructure.Common;
using Microsoft.Extensions.Logging;

namespace BeamOs.StructuralAnalysis.Infrastructure;

internal sealed class UnitOfWork(StructuralAnalysisDbContext dbContext)
    : IStructuralAnalysisUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return dbContext.SaveChangesAsync(ct);
    }
}

internal sealed class SqliteUnitOfWork(
    StructuralAnalysisDbContext dbContext,
    ILogger<SqliteUnitOfWork> logger
) : IStructuralAnalysisUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        Console.WriteLine(
            "Connection interceptor IsLocked: {0}",
            SqliteConnectionLockInterceptor.IsLocked
        );
        // Console.WriteLine("Waiting to save changes...");
        // await SqliteConnectionLockInterceptor.ConnectionLock.WaitAsync(ct);
        // // await semaphore.WaitAsync(ct);
        // try
        // {
        //     Console.WriteLine("Acquired lock to save changes.");
        await dbContext.SaveChangesAsync(ct);
        // }
        // finally
        // {
        //     Console.WriteLine("Releasing lock after saving changes.");
        //     SqliteConnectionLockInterceptor.ConnectionLock.Release();
        // }
    }
}
