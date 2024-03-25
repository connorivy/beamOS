using UnitsNet;

namespace BeamOs.Application.DirectStiffnessMethod.Materials;

public record CreateMaterialCommand(
    string Id,
    Pressure ModulusOfElasticity,
    Pressure ModulusOfRigidity
);
