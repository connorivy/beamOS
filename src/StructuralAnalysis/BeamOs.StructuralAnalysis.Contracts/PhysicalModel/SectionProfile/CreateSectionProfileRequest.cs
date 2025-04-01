using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;

public record CreateSectionProfileRequest : SectionProfileData
{
    public int? Id { get; init; }

    [SetsRequiredMembers]
    public CreateSectionProfileRequest(SectionProfileData sectionProfileData)
        : base(sectionProfileData) { }

    public CreateSectionProfileRequest() { }
}

public record SectionProfileData
{
    public required double Area { get; init; }
    public required double StrongAxisMomentOfInertia { get; init; }
    public required double WeakAxisMomentOfInertia { get; init; }
    public required double PolarMomentOfInertia { get; init; }
    public required double StrongAxisShearArea { get; init; }
    public required double WeakAxisShearArea { get; init; }

    public AreaUnit AreaUnit { get; init; }
    public AreaMomentOfInertiaUnit AreaMomentOfInertiaUnit { get; init; }

    public SectionProfileData() { }

    [SetsRequiredMembers]
    public SectionProfileData(SectionProfileData sectionProfileData)
    {
        this.AreaUnit = sectionProfileData.AreaUnit;
        this.AreaMomentOfInertiaUnit = sectionProfileData.AreaMomentOfInertiaUnit;
        this.Area = sectionProfileData.Area;
        this.StrongAxisMomentOfInertia = sectionProfileData.StrongAxisMomentOfInertia;
        this.WeakAxisMomentOfInertia = sectionProfileData.WeakAxisMomentOfInertia;
        this.PolarMomentOfInertia = sectionProfileData.PolarMomentOfInertia;
        this.StrongAxisShearArea = sectionProfileData.StrongAxisShearArea;
        this.WeakAxisShearArea = sectionProfileData.WeakAxisShearArea;
    }
}

public record PutSectionProfileRequest : SectionProfileData, IHasIntId
{
    public required int Id { get; init; }

    [SetsRequiredMembers]
    public PutSectionProfileRequest(int id, SectionProfileData sectionProfileData)
        : base(sectionProfileData)
    {
        this.Id = id;
    }

    public PutSectionProfileRequest() { }
}
