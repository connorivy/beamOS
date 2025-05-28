namespace BeamOs.StructuralAnalysis.Application.Common;

public interface IStructuralAnalysisUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct = default);
}

public sealed class InMemoryUnitOfWork : IStructuralAnalysisUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await Task.CompletedTask;
    }
}
