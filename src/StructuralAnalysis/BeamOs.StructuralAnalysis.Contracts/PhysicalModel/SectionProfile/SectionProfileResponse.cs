using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;

public record SectionProfileResponse(
    int Id,
    Guid ModelId,
    double Area,
    double StrongAxisMomentOfInertia,
    double WeakAxisMomentOfInertia,
    double PolarMomentOfInertia,
    double StrongAxisShearArea,
    double WeakAxisShearArea,
    AreaUnitContract AreaUnit,
    AreaMomentOfInertiaUnitContract AreaMomentOfInertiaUnit
) : IModelEntity;
