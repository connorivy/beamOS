using BeamOs.Application.PhysicalModel.MomentLoads;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.Repositories.PhysicalModel.MomentLoads;

internal class MomentLoadDbContextRepository(BeamOsStructuralDbContext dbContext)
    : RepositoryBase<MomentLoadId, MomentLoad>(dbContext),
        IMomentLoadRepository
{
    public async Task<List<MomentLoad>> GetMomentLoadsBelongingToNode(NodeId nodeId)
    {
        return await this.DbContext.MomentLoads.Where(m => m.NodeId == nodeId).ToListAsync();
    }

    public async Task<List<MomentLoad>> GetMomentLoadsBelongingToNodes(IList<NodeId> nodeIds)
    {
        return await this.DbContext
            .MomentLoads
            .Where(ml => nodeIds.Contains(ml.NodeId))
            .ToListAsync();
    }

    public Task<List<MomentLoad>> GetMomentLoadsBelongingToNodes(IList<Node> nodes)
    {
        return this.GetMomentLoadsBelongingToNodes(nodes.Select(n => n.Id).ToList());
    }
}
