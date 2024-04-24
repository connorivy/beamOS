using BeamOs.Application.Common.Interfaces;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.Repositories.PhysicalModel.Nodes;

public class NodeDbContextRepository(BeamOsStructuralDbContext dbContext)
    : IRepository<NodeId, Node>
{
    public async Task Add(Node aggregate)
    {
        _ = await dbContext.Nodes.AddAsync(aggregate);
        _ = await dbContext.SaveChangesAsync();
    }

    public async Task<Node?> GetById(NodeId id)
    {
        return await dbContext.Nodes.FirstAsync(el => el.Id == id);
    }
}
