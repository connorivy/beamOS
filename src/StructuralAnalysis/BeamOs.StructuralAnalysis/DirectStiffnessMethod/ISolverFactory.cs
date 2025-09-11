using CSparse.Double.Factorization;
using CSparse.Double.Factorization.MKL;
using CSparse.Factorization;
using CSparse.Storage;

namespace BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;

internal interface ISolverFactory
{
    public ISolver<double> GetSolver(CompressedColumnStorage<double> matrix);
}

internal sealed class PardisoSolverFactory : ISolverFactory
{
    public ISolver<double> GetSolver(CompressedColumnStorage<double> matrix)
    {
        return new Pardiso(matrix);
    }
}

internal sealed class CholeskySolverFactory : ISolverFactory
{
    public ISolver<double> GetSolver(CompressedColumnStorage<double> matrix)
    {
        return SparseCholesky.Create(matrix, CSparse.ColumnOrdering.Natural);
    }
}
