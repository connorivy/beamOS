using BeamOs.Application.Common.Commands;
using UnitsNet;

namespace BeamOs.Application.PhysicalModel.Materials;

public record CreateMaterialCommand(
    GuidBasedIdCommand ModelId,
    Pressure ModulusOfElasticity,
    Pressure ModulusOfRigidity
);
