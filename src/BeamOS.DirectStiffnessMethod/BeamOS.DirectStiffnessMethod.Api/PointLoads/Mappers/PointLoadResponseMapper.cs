using BeamOS.Common.Api.Interfaces;
using BeamOS.Common.Api.Mappers;
using BeamOS.DirectStiffnessMethod.Application.PointLoads;
using BeamOS.PhysicalModel.Contracts.PointLoad;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Api.PointLoads.Mappers;

[Mapper]
[UseStaticMapper(typeof(Vector3ToFromMathnetVector))]
[UseStaticMapper(typeof(UnitValueDtoToForceMapper))]
public partial class PointLoadResponseMapper : IMapper<PointLoadResponse, CreatePointLoadCommand>
{
    public CreatePointLoadCommand Map(PointLoadResponse source) => this.ToCommand(source);
    [MapProperty(nameof(PointLoadResponse.NormalizedDirection), nameof(CreatePointLoadCommand.Direction))]
    public partial CreatePointLoadCommand ToCommand(PointLoadResponse source);
}
