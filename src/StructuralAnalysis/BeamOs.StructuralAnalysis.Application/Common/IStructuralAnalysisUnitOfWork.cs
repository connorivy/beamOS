namespace BeamOs.StructuralAnalysis.Application.Common;

public interface IStructuralAnalysisUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct = default);
}
