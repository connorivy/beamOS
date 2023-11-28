using BeamOS.Common.Api.Interfaces;
using BeamOS.Common.Api.Mappers;
using BeamOS.PhysicalModel.Application.PointLoads.Commands;
using BeamOS.PhysicalModel.Contracts.PointLoad;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Api.PointLoads.Mappers;

[Mapper]
[UseStaticMapper(typeof(Vector3ToFromMathnetVector))]
[UseStaticMapper(typeof(UnitValueDtoToForceMapper))]
public partial class CreatePointLoadRequestMapper : IMapper<CreatePointLoadRequest, CreatePointLoadCommand>
{
    public CreatePointLoadCommand Map(CreatePointLoadRequest from) => this.ToCommand(from);
    public partial CreatePointLoadCommand ToCommand(CreatePointLoadRequest request);
}
