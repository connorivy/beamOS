using BeamOS.Common.Api.Interfaces;
using BeamOS.Common.Contracts;
using BeamOS.DirectStiffnessMethod.Application.Materials;
using BeamOS.PhysicalModel.Contracts.Material;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.DirectStiffnessMethod.Api.AnalyticalModels.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitValueDTOToPressureMapper))]
public partial class MaterialResponseToCreateMaterialCommand : IMapper<MaterialResponse, CreateMaterialCommand>
{
    public CreateMaterialCommand Map(MaterialResponse from) => this.ToCommand(from);
    public partial CreateMaterialCommand ToCommand(MaterialResponse from);
}

[Mapper]
public static partial class StringToPressureUnitMapper
{
    public static partial PressureUnit MapToPressureUnit(this string unit);
}

public static class UnitValueDTOToPressureMapper
{
    public static Pressure MapToForce(this UnitValueDTO dto)
    {
        return new(dto.Value, dto.Unit.MapToPressureUnit());
    }
}
