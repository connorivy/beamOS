using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.Material;

public record MaterialResponse(
    string Id,
    string ModelId,
    PressureContract ModulusOfElasticity,
    PressureContract ModulusOfRigidity
);
