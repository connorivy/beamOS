using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace BeamOs.StructuralAnalysis.Sdk;

public class AsyncGuidLockManager : IDisposable
{
    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _semaphores = new();

    private readonly Lock _lock = new Lock();

    // Gets or creates a SemaphoreSlim for the specified GUID
    private SemaphoreSlim GetSemaphore(Guid id)
    {
        lock (_lock)
        {
            return _semaphores.GetOrAdd(id, _ => new SemaphoreSlim(1, 1));
        }
    }

    // Executes async code with a GUID-specific lock
    public async Task<T> ExecuteWithLockAsync<T>(Guid id, Func<Task<T>> taskFactory)
    {
        var semaphore = GetSemaphore(id);
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
        var semaphore = GetSemaphore(id);
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
        foreach (var pair in _semaphores)
        {
            // Remove semaphore if no one is waiting and current count is 1 (available)
            if (pair.Value.CurrentCount == 1)
            {
                _semaphores.TryRemove(pair.Key, out _);
                pair.Value.Dispose();
            }
        }
    }

    public void Dispose()
    {
        foreach (var semaphore in _semaphores.Values)
        {
            semaphore.Dispose();
        }
        _semaphores.Clear();
    }
}
