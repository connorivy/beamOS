using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;

public record CreateSectionProfileRequest : SectionProfileData
{
    public int? Id { get; init; }

    [SetsRequiredMembers]
    public CreateSectionProfileRequest(SectionProfileData sectionProfileData)
        : base(sectionProfileData) { }

    public CreateSectionProfileRequest() { }
}

public record SectionProfileData : SectionProfileDataBase
{
    public required double Area { get; init; }
    public required double StrongAxisMomentOfInertia { get; init; }
    public required double WeakAxisMomentOfInertia { get; init; }
    public required double PolarMomentOfInertia { get; init; }
    public required double StrongAxisPlasticSectionModulus { get; init; }
    public required double WeakAxisPlasticSectionModulus { get; init; }
    public double? StrongAxisShearArea { get; init; }
    public double? WeakAxisShearArea { get; init; }

    public required LengthUnitContract LengthUnit { get; init; }

    [JsonIgnore]
    public VolumeUnitContract VolumeUnit => this.LengthUnit.ToVolume();

    [JsonIgnore]
    public AreaUnitContract AreaUnit => this.LengthUnit.ToArea();

    [JsonIgnore]
    public AreaMomentOfInertiaUnitContract AreaMomentOfInertiaUnit =>
        this.LengthUnit.ToAreaMomentOfInertia();

    public SectionProfileData() { }

    [SetsRequiredMembers]
    public SectionProfileData(SectionProfileData sectionProfileData)
        : base(sectionProfileData)
    {
        this.LengthUnit = sectionProfileData.LengthUnit;
        this.Area = sectionProfileData.Area;
        this.StrongAxisMomentOfInertia = sectionProfileData.StrongAxisMomentOfInertia;
        this.WeakAxisMomentOfInertia = sectionProfileData.WeakAxisMomentOfInertia;
        this.PolarMomentOfInertia = sectionProfileData.PolarMomentOfInertia;
        this.StrongAxisPlasticSectionModulus = sectionProfileData.StrongAxisPlasticSectionModulus;
        this.WeakAxisPlasticSectionModulus = sectionProfileData.WeakAxisPlasticSectionModulus;
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

public abstract record SectionProfileDataBase
{
    public required string Name { get; init; }

    public SectionProfileDataBase() { }

    [SetsRequiredMembers]
    public SectionProfileDataBase(string name)
    {
        this.Name = name;
    }

    [SetsRequiredMembers]
    public SectionProfileDataBase(SectionProfileDataBase sectionProfileDataBase)
    {
        this.Name = sectionProfileDataBase.Name;
    }
}

public record SectionProfileFromLibraryData : SectionProfileDataBase
{
    public required StructuralCode Library { get; init; }

    [SetsRequiredMembers]
    public SectionProfileFromLibraryData(StructuralCode library, string name)
        : base(name)
    {
        this.Library = library;
    }

    public SectionProfileFromLibraryData() { }
}

public enum StructuralCode
{
    Undefined = 0,
    AISC_360_16,
}

public record CreateSectionProfileFromLibraryRequest : SectionProfileFromLibraryData, IHasIntId
{
    public required int Id { get; init; }

    [SetsRequiredMembers]
    public CreateSectionProfileFromLibraryRequest(int id, StructuralCode library, string name)
        : base(library, name)
    {
        this.Id = id;
    }

    public CreateSectionProfileFromLibraryRequest() { }
}

public record SectionProfileFromLibrary : SectionProfileFromLibraryData, IHasIntId
{
    public required int Id { get; init; }

    [SetsRequiredMembers]
    public SectionProfileFromLibrary(int id, StructuralCode library, string name)
        : base(library, name)
    {
        this.Id = id;
    }

    public SectionProfileFromLibrary() { }
}
