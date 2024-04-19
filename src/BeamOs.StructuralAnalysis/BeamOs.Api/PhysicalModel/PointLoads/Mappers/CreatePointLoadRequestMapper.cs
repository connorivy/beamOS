using BeamOs.Api.Common.Interfaces;
using BeamOs.Api.Common.Mappers;
using BeamOs.Application.PhysicalModel.PointLoads.Commands;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.PointLoads.Mappers;

[Mapper]
[UseStaticMapper(typeof(Vector3ToFromMathnetVector3d))]
[UseStaticMapper(typeof(UnitValueDtoToForceMapper))]
public partial class CreatePointLoadRequestMapper
    : IMapper<CreatePointLoadRequest, CreatePointLoadCommand>
{
    public CreatePointLoadCommand Map(CreatePointLoadRequest from) => this.ToCommand(from);

    public partial CreatePointLoadCommand ToCommand(CreatePointLoadRequest request);
}
