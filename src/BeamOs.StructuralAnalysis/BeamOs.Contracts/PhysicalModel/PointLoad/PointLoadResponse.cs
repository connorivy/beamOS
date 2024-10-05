using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Common;

namespace BeamOs.Contracts.PhysicalModel.PointLoad;

public record PointLoadResponse(
    string Id,
    string ModelId,
    string NodeId,
    ForceContract Force,
    Vector3 Direction
) : BeamOsEntityContractBase(Id);
