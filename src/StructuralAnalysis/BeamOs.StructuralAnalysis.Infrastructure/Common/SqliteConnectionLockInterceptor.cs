using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BeamOs.StructuralAnalysis.Infrastructure.Common;

internal sealed class SqliteConnectionLockInterceptor : DbCommandInterceptor, IDisposable
{
    internal static SemaphoreSlim ConnectionLock { get; } = new(1, 1);
    private static readonly Lock connectionLock = new();
    internal static bool IsLocked => ConnectionLock.CurrentCount > 1;

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result
    ) => throw new NotImplementedException("Use ReaderExecutingAsync instead.");

    public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine("Waiting for lock...");
        await ConnectionLock.WaitAsync(cancellationToken);

        Console.WriteLine("Acquired lock.");
        return await base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override async ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine("Waiting for lock...");
        await ConnectionLock.WaitAsync(cancellationToken);

        Console.WriteLine("Acquired lock.");
        return await base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }

    // public override InterceptionResult<int> NonQueryExecuting(
    //     DbCommand command,
    //     CommandEventData eventData,
    //     InterceptionResult<int> result
    // ) => throw new NotImplementedException("Use NonQueryExecutingAsync instead.");

    public override async ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine("Waiting for lock...");
        await ConnectionLock.WaitAsync(cancellationToken);

        Console.WriteLine("Acquired lock.");
        return await base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override Task CommandCanceledAsync(
        DbCommand command,
        CommandEndEventData eventData,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine("Releasing lock due to command cancellation.");
        ConnectionLock.Release();
        return base.CommandCanceledAsync(command, eventData, cancellationToken);
    }

    public override Task CommandFailedAsync(
        DbCommand command,
        CommandErrorEventData eventData,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine("Releasing lock due to command failure.");
        ConnectionLock.Release();
        return base.CommandFailedAsync(command, eventData, cancellationToken);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine("Releasing lock.");
        ConnectionLock.Release();
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    public void Dispose()
    {
        ConnectionLock.Dispose();
    }
}

internal sealed class SqliteSavingChangesConnectionLockInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine("Waiting for lock to save changes...");
        // await SqliteConnectionLockInterceptor.ConnectionLock.WaitAsync(cancellationToken);
        Console.WriteLine("Acquired lock to save changes.");
        // try
        // {
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
        // }
        // finally
        // {
        //     Console.WriteLine("Releasing lock after preparing to save changes.");
        //     SqliteConnectionLockInterceptor.ConnectionLock.Release();
        // }
    }

    public override Task SaveChangesCanceledAsync(
        DbContextEventData eventData,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine("Releasing lock due to save changes cancellation.");
        // SqliteConnectionLockInterceptor.ConnectionLock.Release();
        return base.SaveChangesCanceledAsync(eventData, cancellationToken);
    }

    public override Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine("Releasing lock due to save changes failure.");
        // SqliteConnectionLockInterceptor.ConnectionLock.Release();
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    public override ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine("Releasing lock after saving changes.");
        // SqliteConnectionLockInterceptor.ConnectionLock.Release();
        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
