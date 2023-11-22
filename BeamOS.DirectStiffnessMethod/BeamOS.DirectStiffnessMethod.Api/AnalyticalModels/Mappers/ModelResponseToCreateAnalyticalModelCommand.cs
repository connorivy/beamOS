using BeamOS.Common.Api.Interfaces;
using BeamOS.Common.Contracts;
using BeamOS.DirectStiffnessMethod.Api.AnalyticalNodes.Mappers;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalModels.Commands;
using BeamOS.PhysicalModel.Contracts.Model;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.DirectStiffnessMethod.Api.AnalyticalModels.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitValueDTOToAngleMapper))]
[UseStaticMapper(typeof(UnitValueDTOToLengthMapper))]
public partial class ModelResponseToCreateAnalyticalModelCommand : IMapper<ModelResponse, CreateAnalyticalModelCommand>
{
    public CreateAnalyticalModelCommand Map(ModelResponse from) => throw new NotImplementedException();
    public partial CreateAnalyticalModelCommand ToCommand(ModelResponse from);
}

[Mapper]
public static partial class StringToAngleUnitMapper
{
    public static partial AngleUnit MapToAngleUnit(this string unit);
}

public static class UnitValueDTOToAngleMapper
{
    public static Angle MapToForce(this UnitValueDTO dto)
    {
        return new(dto.Value, dto.Unit.MapToAngleUnit());
    }
}
