using BeamOS.Common.Contracts;

namespace BeamOS.PhysicalModel.Contracts.Material;

public record CreateMaterialRequest(
    string ModelId,
    UnitValueDTO ModulusOfElasticity,
    UnitValueDTO ModulusOfRigidity
);
