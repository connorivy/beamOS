using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Application.Material;
public record CreateMaterialCommand(Pressure ModulusOfElasticity, Pressure ModulusOfRigidity);
