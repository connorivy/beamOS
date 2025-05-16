using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;

public record MaterialResponse(
    int Id,
    Guid ModelId,
    double ModulusOfElasticity,
    double ModulusOfRigidity,
    PressureUnit PressureUnit
) : IModelEntity;
