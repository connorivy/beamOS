using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;

// public record PointLoadResponse(int Id, int NodeId, Guid ModelId, Force Force, Vector3 Direction)
//     : IModelEntity
// {
//     public PointLoadData ToPointLoadData() => new(this.NodeId, this.Force, this.Direction);
// }

public record PointLoadResponse : PointLoad, IModelEntity
{
    public required Guid ModelId { get; init; }

    [SetsRequiredMembers]
    public PointLoadResponse(
        int id,
        int nodeId,
        int loadCaseId,
        Guid modelId,
        ForceContract force,
        Vector3 direction
    )
        : base(id, nodeId, loadCaseId, force, direction)
    {
        this.ModelId = modelId;
    }
}
