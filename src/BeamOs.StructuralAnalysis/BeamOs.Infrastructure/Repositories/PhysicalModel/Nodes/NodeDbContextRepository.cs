using BeamOs.Application.PhysicalModel.Nodes.Interfaces;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.Repositories.PhysicalModel.Nodes;

internal class NodeDbContextRepository(BeamOsStructuralDbContext dbContext)
    : RepositoryBase<NodeId, Node>(dbContext),
        INodeRepository
{
    public async Task<List<Node>> GetNodesInModel(ModelId modelId, CancellationToken ct = default)
    {
        return await this.DbContext.Nodes.Where(n => n.ModelId == modelId).ToListAsync(ct);
    }
}
