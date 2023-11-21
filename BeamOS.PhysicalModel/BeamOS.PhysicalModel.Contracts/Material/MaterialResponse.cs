using BeamOS.Common.Contracts;

namespace BeamOS.PhysicalModel.Contracts.Material;
public record MaterialResponse(
    UnitValueDTO ModulusOfElasticity,
    UnitValueDTO ModulusOfRigidity);
