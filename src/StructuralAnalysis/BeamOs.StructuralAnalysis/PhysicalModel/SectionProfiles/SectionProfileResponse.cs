using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;

public record SectionProfileResponse(
    int Id,
    Guid ModelId,
    string Name,
    double Area,
    double StrongAxisMomentOfInertia,
    double WeakAxisMomentOfInertia,
    double PolarMomentOfInertia,
    double StrongAxisPlasticSectionModulus,
    double WeakAxisPlasticSectionModulus,
    double? StrongAxisShearArea,
    double? WeakAxisShearArea,
    LengthUnitContract LengthUnit
) : IModelEntity
{
    public SectionProfileData ToSectionProfileData() =>
        new()
        {
            LengthUnit = this.LengthUnit,
            Name = this.Name,
            Area = this.Area,
            StrongAxisMomentOfInertia = this.StrongAxisMomentOfInertia,
            WeakAxisMomentOfInertia = this.WeakAxisMomentOfInertia,
            PolarMomentOfInertia = this.PolarMomentOfInertia,
            StrongAxisPlasticSectionModulus = this.StrongAxisPlasticSectionModulus,
            WeakAxisPlasticSectionModulus = this.WeakAxisPlasticSectionModulus,
            StrongAxisShearArea = this.StrongAxisShearArea,
            WeakAxisShearArea = this.WeakAxisShearArea,
        };

    [JsonIgnore]
    [IgnoreDataMember]
    public VolumeUnitContract VolumeUnit => this.LengthUnit.ToVolume();

    [JsonIgnore]
    [IgnoreDataMember]
    public AreaUnitContract AreaUnit => this.LengthUnit.ToArea();

    [JsonIgnore]
    [IgnoreDataMember]
    public AreaMomentOfInertiaUnitContract AreaMomentOfInertiaUnit =>
        this.LengthUnit.ToAreaMomentOfInertia();
}
