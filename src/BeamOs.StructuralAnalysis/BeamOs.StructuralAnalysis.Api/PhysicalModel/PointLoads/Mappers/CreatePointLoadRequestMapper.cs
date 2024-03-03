using BeamOs.Api.Common.Interfaces;
using BeamOs.Api.Common.Mappers;
using BeamOS.PhysicalModel.Application.PointLoads.Commands;
using BeamOS.PhysicalModel.Contracts.PointLoad;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.PointLoads.Mappers;

[Mapper]
[UseStaticMapper(typeof(Vector3ToFromMathnetVector))]
[UseStaticMapper(typeof(UnitValueDtoToForceMapper))]
public partial class CreatePointLoadRequestMapper
    : IMapper<CreatePointLoadRequest, CreatePointLoadCommand>
{
    public CreatePointLoadCommand Map(CreatePointLoadRequest from) => this.ToCommand(from);

    public partial CreatePointLoadCommand ToCommand(CreatePointLoadRequest request);
}
