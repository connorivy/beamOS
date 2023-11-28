using BeamOS.Common.Api.Interfaces;
using BeamOS.Common.Api.Mappers;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalNodes.Commands;
using BeamOS.PhysicalModel.Contracts.Node;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Api.AnalyticalNodes.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitValueDtoToLengthMapper))]
public partial class PointResponseMapper : IMapper<PointResponse, PointCommand>
{
    public PointCommand Map(PointResponse from) => this.ToCommand(from);
    public partial PointCommand ToCommand(PointResponse response);
}
