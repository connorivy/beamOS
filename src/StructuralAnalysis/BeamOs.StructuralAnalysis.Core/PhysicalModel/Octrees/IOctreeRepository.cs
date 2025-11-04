using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

internal interface IOctreeRepository : IRepository<OctreeId, Octree>
{
    public Task<Result> AddNodeIfWithinTolerance(
        ModelId modelId,
        NodeDefinition node,
        CancellationToken ct = default
    );
}

internal interface IOctreeNodeRepository : IRepository<OctreeNodeId, OctreeNode>
{
    public Task<Result> AddNodeIfWithinTolerance(
        ModelId modelId,
        NodeDefinition node,
        CancellationToken ct = default
    );
}
