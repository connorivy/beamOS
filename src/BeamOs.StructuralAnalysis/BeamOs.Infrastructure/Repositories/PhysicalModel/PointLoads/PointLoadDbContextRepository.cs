using BeamOs.Application.PhysicalModel.PointLoads;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.Repositories.PhysicalModel.PointLoads;

internal class PointLoadDbContextRepository(BeamOsStructuralDbContext dbContext)
    : RepositoryBase<PointLoadId, PointLoad>(dbContext),
        IPointLoadRepository
{
    public async Task<List<PointLoad>> GetPointLoadsBelongingToNode(NodeId nodeId)
    {
        return await this.DbContext.PointLoads.Where(pl => pl.NodeId == nodeId).ToListAsync();
    }

    public async Task<List<PointLoad>> GetPointLoadsBelongingToNodes(IList<NodeId> nodeIds)
    {
        return await this.DbContext
            .PointLoads
            .Where(pl => nodeIds.Contains(pl.NodeId))
            .ToListAsync();
    }

    public Task<List<PointLoad>> GetPointLoadsBelongingToNodes(IList<Node> nodes)
    {
        return this.GetPointLoadsBelongingToNodes(nodes.Select(n => n.Id).ToList());
    }
}
