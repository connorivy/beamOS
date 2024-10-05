using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.Material;

public record CreateMaterialRequest(
    string ModelId,
    PressureContract ModulusOfElasticity,
    PressureContract ModulusOfRigidity
);
