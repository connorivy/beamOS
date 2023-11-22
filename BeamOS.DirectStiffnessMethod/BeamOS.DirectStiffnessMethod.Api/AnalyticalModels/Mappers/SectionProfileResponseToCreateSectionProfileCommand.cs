using BeamOS.Common.Api.Interfaces;
using BeamOS.Common.Contracts;
using BeamOS.DirectStiffnessMethod.Application.SectionProfiles;
using BeamOS.PhysicalModel.Contracts.SectionProfile;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.DirectStiffnessMethod.Api.AnalyticalModels.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitValueDTOToAreaMapper))]
[UseStaticMapper(typeof(UnitValueDTOToAreaMomentOfInertiaMapper))]
public partial class SectionProfileResponseToCreateSectionProfileCommand
    : IMapper<SectionProfileResponse, CreateSectionProfileCommand>
{
    public CreateSectionProfileCommand Map(SectionProfileResponse from) => this.ToCommand(from);
    public partial CreateSectionProfileCommand ToCommand(SectionProfileResponse from);
}

[Mapper]
public static partial class StringToAreaUnitMapper
{
    public static partial AreaUnit MapToAreaUnit(this string unit);
}

public static class UnitValueDTOToAreaMapper
{
    public static Area MapToForce(this UnitValueDTO dto)
    {
        return new(dto.Value, dto.Unit.MapToAreaUnit());
    }
}


[Mapper]
public static partial class StringToAreaMomentOfInertiaUnitMapper
{
    public static partial AreaMomentOfInertiaUnit MapToAreaMomentOfInertiaUnit(this string unit);
}

public static class UnitValueDTOToAreaMomentOfInertiaMapper
{
    public static AreaMomentOfInertia MapToForce(this UnitValueDTO dto)
    {
        return new(dto.Value, dto.Unit.MapToAreaMomentOfInertiaUnit());
    }
}
