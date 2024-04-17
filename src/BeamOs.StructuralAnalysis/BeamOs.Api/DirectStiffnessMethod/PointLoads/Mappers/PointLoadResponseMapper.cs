using BeamOs.Api.Common.Mappers;
using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.DirectStiffnessMethod.PointLoads;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.DirectStiffnessMethod.PointLoads.Mappers;

[Mapper]
[UseStaticMapper(typeof(Vector3ToFromMathnetVector))]
[UseStaticMapper(typeof(UnitValueDtoToForceMapper))]
public partial class PointLoadResponseMapper : IMapper<PointLoadResponse, CreatePointLoadCommand>
{
    public CreatePointLoadCommand Map(PointLoadResponse source) => this.ToCommand(source);

    [MapProperty(
        nameof(PointLoadResponse.NormalizedDirection),
        nameof(CreatePointLoadCommand.Direction)
    )]
    public partial CreatePointLoadCommand ToCommand(PointLoadResponse source);
}
