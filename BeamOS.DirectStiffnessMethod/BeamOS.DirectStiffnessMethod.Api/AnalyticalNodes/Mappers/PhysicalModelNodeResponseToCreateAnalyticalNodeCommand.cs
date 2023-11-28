using BeamOS.Common.Api.Interfaces;
using BeamOS.Common.Contracts;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalNodes.Commands;
using BeamOS.PhysicalModel.Contracts.Node;
using Riok.Mapperly.Abstractions;
using UnitsNet.Units;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Api.AnalyticalNodes.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitValueDTOToLengthMapper))]
public partial class PhysicalModelNodeResponseToCreateAnalyticalNodeCommand : IMapper<NodeResponse, CreateAnalyticalNodeCommand>
{
    public CreateAnalyticalNodeCommand Map(NodeResponse from) => throw new NotImplementedException();
    public partial CreateAnalyticalNodeCommand ToResponse(NodeResponse model);
}

[Mapper]
public static partial class StringToLengthUnitMapper
{
    public static partial LengthUnit MapToLengthUnit(this string unit);
}

public static class UnitValueDTOToLengthMapper
{
    public static Length MapToForce(this UnitValueDTO dto)
    {
        return new(dto.Value, dto.Unit.MapToLengthUnit());
    }
}
