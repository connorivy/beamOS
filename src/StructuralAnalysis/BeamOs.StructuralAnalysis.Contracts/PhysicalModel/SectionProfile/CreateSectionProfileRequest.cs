using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;

public record CreateSectionProfileRequest
{
    public AreaContract Area { get; init; }
    public AreaMomentOfInertiaContract StrongAxisMomentOfInertia { get; init; }
    public AreaMomentOfInertiaContract WeakAxisMomentOfInertia { get; init; }
    public AreaMomentOfInertiaContract PolarMomentOfInertia { get; init; }
    public AreaContract StrongAxisShearArea { get; init; }
    public AreaContract WeakAxisShearArea { get; init; }
    public int? Id { get; init; }

    [SetsRequiredMembers]
    public CreateSectionProfileRequest(
        AreaContract area,
        AreaMomentOfInertiaContract strongAxisMomentOfInertia,
        AreaMomentOfInertiaContract weakAxisMomentOfInertia,
        AreaMomentOfInertiaContract polarMomentOfInertia,
        AreaContract strongAxisShearArea,
        AreaContract weakAxisShearArea,
        int? id = null
    )
    {
        this.Area = area;
        this.StrongAxisMomentOfInertia = strongAxisMomentOfInertia;
        this.WeakAxisMomentOfInertia = weakAxisMomentOfInertia;
        this.PolarMomentOfInertia = polarMomentOfInertia;
        this.StrongAxisShearArea = strongAxisShearArea;
        this.WeakAxisShearArea = weakAxisShearArea;
        this.Id = id;
    }

    public CreateSectionProfileRequest() { }
}

public record SectionProfileRequestData
{
    public AreaContract Area { get; init; }
    public AreaMomentOfInertiaContract StrongAxisMomentOfInertia { get; init; }
    public AreaMomentOfInertiaContract WeakAxisMomentOfInertia { get; init; }
    public AreaMomentOfInertiaContract PolarMomentOfInertia { get; init; }
    public AreaContract StrongAxisShearArea { get; init; }
    public AreaContract WeakAxisShearArea { get; init; }

    [SetsRequiredMembers]
    public SectionProfileRequestData(
        AreaContract area,
        AreaMomentOfInertiaContract strongAxisMomentOfInertia,
        AreaMomentOfInertiaContract weakAxisMomentOfInertia,
        AreaMomentOfInertiaContract polarMomentOfInertia,
        AreaContract strongAxisShearArea,
        AreaContract weakAxisShearArea
    )
    {
        this.Area = area;
        this.StrongAxisMomentOfInertia = strongAxisMomentOfInertia;
        this.WeakAxisMomentOfInertia = weakAxisMomentOfInertia;
        this.PolarMomentOfInertia = polarMomentOfInertia;
        this.StrongAxisShearArea = strongAxisShearArea;
        this.WeakAxisShearArea = weakAxisShearArea;
    }

    public SectionProfileRequestData() { }
}

public record PutSectionProfileRequest : SectionProfileRequestData, IHasIntId
{
    public required int Id { get; init; }
}
