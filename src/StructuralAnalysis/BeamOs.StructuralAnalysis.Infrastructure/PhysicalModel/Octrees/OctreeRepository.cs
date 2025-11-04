using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Infrastructure;
using BeamOs.StructuralAnalysis.Infrastructure.Common;

internal sealed class OctreeRepository(StructuralAnalysisDbContext dbContext)
    : RepositoryBase<OctreeId, Octree>(dbContext),
        IOctreeRepository
{
    public Task<Result> AddNodeIfWithinTolerance(
        ModelId modelId,
        NodeDefinition node,
        CancellationToken ct = default
    ) => throw new NotImplementedException();
}
