using BeamOS.Common.Contracts;

namespace BeamOS.PhysicalModel.Contracts.Material;

public record MaterialResponse(
    string Id,
    UnitValueDTO ModulusOfElasticity,
    UnitValueDTO ModulusOfRigidity
);
