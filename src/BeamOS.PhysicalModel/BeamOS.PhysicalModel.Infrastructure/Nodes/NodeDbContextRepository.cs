using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Domain.NodeAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOS.PhysicalModel.Infrastructure.Nodes;

public class NodeDbContextRepository(PhysicalModelDbContext dbContext) : IRepository<NodeId, Node>
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
