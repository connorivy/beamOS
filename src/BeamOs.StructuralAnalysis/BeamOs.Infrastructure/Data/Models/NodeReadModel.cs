using BeamOs.Application.PhysicalModel.Nodes.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Infrastructure.Data.Models;

internal sealed class NodeReadModel : ReadModelBase, INodeData
{
    public Guid ModelId { get; private set; }
    public Point LocationPoint { get; private set; }
    public Restraint Restraint { get; private set; }
    public List<PointLoadReadModel>? PointLoads { get; private set; }
    public List<MomentLoadReadModel>? MomentLoads { get; private set; }
}
