using BeamOs.Application.Common.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Application.PhysicalModel.Nodes.Interfaces;

public interface INodeData : IEntityData
{
    public Guid ModelId { get; }
    public Point LocationPoint { get; }
    public Restraint Restraint { get; }
    //public List<PointLoadReadModel>? PointLoads { get; }
    //public List<MomentLoadReadModel>? MomentLoads { get; }
}
