using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;

public record MaterialResponse(
    int Id,
    Guid ModelId,
    double ModulusOfElasticity,
    double ModulusOfRigidity,
    PressureUnitContract PressureUnit
) : IModelEntity;
