using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Application.Materials;
public record CreateMaterialCommand(
    string Id,
    Pressure ModulusOfElasticity,
    Pressure ModulusOfRigidity);
