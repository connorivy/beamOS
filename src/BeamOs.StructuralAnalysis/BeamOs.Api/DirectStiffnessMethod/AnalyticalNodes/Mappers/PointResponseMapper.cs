using BeamOs.Api.Common.Interfaces;
using BeamOs.Api.Common.Mappers;
using BeamOs.Application.DirectStiffnessMethod.AnalyticalNodes.Commands;
using BeamOs.Contracts.PhysicalModel.Node;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Api.AnalyticalNodes.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitValueDtoToLengthMapper))]
public partial class PointResponseMapper : IMapper<PointResponse, PointCommand>
{
    public PointCommand Map(PointResponse from) => this.ToCommand(from);

    public partial PointCommand ToCommand(PointResponse response);
}
