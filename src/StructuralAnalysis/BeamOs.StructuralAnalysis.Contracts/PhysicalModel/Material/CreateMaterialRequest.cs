using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;

public record CreateMaterialRequest(
    PressureContract ModulusOfElasticity,
    PressureContract ModulusOfRigidity
);
