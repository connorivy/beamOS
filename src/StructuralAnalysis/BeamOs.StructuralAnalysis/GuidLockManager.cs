using System.Collections.Concurrent;

namespace BeamOs.StructuralAnalysis.Sdk;

internal sealed class AsyncGuidLockManager : IDisposable
{
    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> semaphores = new();

    private readonly Lock @lock = new();

    // Gets or creates a SemaphoreSlim for the specified GUID
    private SemaphoreSlim GetSemaphore(Guid id)
    {
        lock (this.@lock)
        {
            return this.semaphores.GetOrAdd(id, _ => new SemaphoreSlim(1, 1));
        }
    }

    // Executes async code with a GUID-specific lock
    public async Task<T> ExecuteWithLockAsync<T>(Guid id, Func<Task<T>> taskFactory)
    {
        var semaphore = this.GetSemaphore(id);
        await semaphore.WaitAsync().ConfigureAwait(false);

        try
        {
            return await taskFactory().ConfigureAwait(false);
        }
        finally
        {
            semaphore.Release();
        }
    }

    // Executes async action with a GUID-specific lock
    public async Task ExecuteWithLockAsync(Guid id, Func<Task> task)
    {
        var semaphore = this.GetSemaphore(id);
        await semaphore.WaitAsync().ConfigureAwait(false);

        try
        {
            await task().ConfigureAwait(false);
        }
        finally
        {
            semaphore.Release();
        }
    }

    // Cleanup unused semaphores to prevent memory leaks
    public void CleanupUnusedSemaphores()
    {
        foreach (var pair in this.semaphores)
        {
            // Remove semaphore if no one is waiting and current count is 1 (available)
            if (pair.Value.CurrentCount == 1)
            {
                this.semaphores.TryRemove(pair.Key, out _);
                pair.Value.Dispose();
            }
        }
    }

    public void Dispose()
    {
        foreach (var semaphore in this.semaphores.Values)
        {
            semaphore.Dispose();
        }
        this.semaphores.Clear();
    }
}
