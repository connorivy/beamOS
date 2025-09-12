namespace BeamOs.StructuralAnalysis.Application.Common;

internal interface IStructuralAnalysisUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct = default);
}

internal sealed class InMemoryUnitOfWork : IStructuralAnalysisUnitOfWork
{
    public List<Exception> SimulatedFailures { get; } = [];

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        if (this.SimulatedFailures.Count > 0)
        {
            throw this.SimulatedFailures[0];
        }
        await Task.CompletedTask;
    }
}
