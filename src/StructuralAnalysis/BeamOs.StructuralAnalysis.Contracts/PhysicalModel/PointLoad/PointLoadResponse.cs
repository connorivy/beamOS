using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;

public record PointLoadResponse(int Id, int NodeId, Guid ModelId, Force Force, Vector3 Direction)
    : IModelEntity
{
    public PointLoadData ToPointLoadData() => new(this.NodeId, this.Force, this.Direction);
}
