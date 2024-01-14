using BeamOS.Common.Application.Commands;
using UnitsNet;

namespace BeamOS.PhysicalModel.Application.Materials;

public record CreateMaterialCommand(
    GuidBasedIdCommand ModelId,
    Pressure ModulusOfElasticity,
    Pressure ModulusOfRigidity
);
