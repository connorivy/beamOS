using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Domain.NodeAggregate.ValueObjects;

namespace BeamOS.PhysicalModel.Infrastructure.Nodes;
public class NodeDbContextRepository(PhysicalModelDbContext dbContext) : IRepository<NodeId, Node>
{
    public async Task Add(Node aggregate)
    {
        _ = await dbContext.Nodes.AddAsync(aggregate);
        _ = await dbContext.SaveChangesAsync();
    }
    public Task<Node?> GetById(NodeId id)
    {
        return Task.FromResult(dbContext.Nodes
            .Where(el => el.Id == id)
            .FirstOrDefault());
    }
}
