using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Infrastructure;
using BeamOs.StructuralAnalysis.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

internal sealed class OctreeRepository(StructuralAnalysisDbContext dbContext)
    : RepositoryBase<OctreeId, Octree>(dbContext),
        IOctreeRepository
{
    private const double ToleranceInches = 1.0;
    private const double ToleranceMeters = ToleranceInches * 0.0254; // 1 inch = 0.0254 meters

    public async Task<Result> AddNodeIfWithinTolerance(
        ModelId modelId,
        NodeDefinition node,
        CancellationToken ct = default
    )
    {
        // Get or create the octree for this model
        var octree = await dbContext.Octrees
            .Include(o => o.Root)
            .FirstOrDefaultAsync(o => o.ModelId == modelId, ct);

        if (octree == null)
        {
            // Create a new octree for this model (will be saved with the unit of work)
            octree = new Octree(modelId);
            dbContext.Octrees.Add(octree);
            // We don't call SaveChanges here - the UnitOfWork will handle that
        }

        // Get the location point for the node
        Point locationPoint = node.GetLocationPoint();

        // Check if there are any nodes within tolerance
        var nearbyNodes = octree.FindNodeIdsWithin(locationPoint, ToleranceMeters);

        if (nearbyNodes.Count > 0)
        {
            return BeamOsError.Conflict(
                description: $"A node already exists within {ToleranceInches} inch(es) of the specified location."
            );
        }

        // Add the node to the octree
        if (node is Node spatialNode)
        {
            octree.Add(spatialNode);
        }
        else if (node is InternalNode internalNode)
        {
            // For internal nodes, we need the element and node stores
            // Since we don't have them here, we'll skip adding internal nodes to the octree for now
            // This is a limitation that should be addressed if internal nodes need duplicate checking
        }

        return Result.Success;
    }
}
