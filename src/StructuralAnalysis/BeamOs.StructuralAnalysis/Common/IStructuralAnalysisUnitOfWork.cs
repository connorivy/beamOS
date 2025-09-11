namespace BeamOs.StructuralAnalysis.Application.Common;

internal interface IStructuralAnalysisUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct = default);
}

internal sealed class InMemoryUnitOfWork : IStructuralAnalysisUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await Task.CompletedTask;
    }
}
