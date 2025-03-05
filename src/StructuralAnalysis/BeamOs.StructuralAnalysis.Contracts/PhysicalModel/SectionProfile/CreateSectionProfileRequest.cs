using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;

public record CreateSectionProfileRequest : SectionProfileRequestData
{
    public int? Id { get; init; }
}

public record SectionProfileRequestData
{
    public double Area { get; init; }
    public double StrongAxisMomentOfInertia { get; init; }
    public double WeakAxisMomentOfInertia { get; init; }
    public double PolarMomentOfInertia { get; init; }
    public double StrongAxisShearArea { get; init; }
    public double WeakAxisShearArea { get; init; }

    public AreaUnit AreaUnit { get; init; }
    public AreaMomentOfInertiaUnit AreaMomentOfInertiaUnit { get; init; }

    public SectionProfileRequestData() { }
}

public record PutSectionProfileRequest : SectionProfileRequestData, IHasIntId
{
    public required int Id { get; init; }
}
