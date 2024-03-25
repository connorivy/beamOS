using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.Material;

public record MaterialResponse(
    string Id,
    UnitValueDto ModulusOfElasticity,
    UnitValueDto ModulusOfRigidity
);
