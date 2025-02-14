namespace BeamOs.StructuralAnalysis.Application.Common;

public interface IStructuralAnalysisUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct = default);
}
