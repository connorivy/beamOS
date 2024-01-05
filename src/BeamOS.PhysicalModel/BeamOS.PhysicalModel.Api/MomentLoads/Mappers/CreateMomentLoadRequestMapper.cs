using BeamOS.Common.Api.Interfaces;
using BeamOS.Common.Api.Mappers;
using BeamOS.PhysicalModel.Application.MomentLoads;
using BeamOS.PhysicalModel.Contracts.MomentLoad;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Api.MomentLoads.Mappers;

[Mapper]
[UseStaticMapper(typeof(Vector3ToFromMathnetVector))]
[UseStaticMapper(typeof(UnitValueDtoToTorqueMapper))]
public partial class CreateMomentLoadRequestMapper
    : IMapper<CreateMomentLoadRequest, CreateMomentLoadCommand>
{
    public CreateMomentLoadCommand Map(CreateMomentLoadRequest from) => this.ToCommand(from);

    public partial CreateMomentLoadCommand ToCommand(CreateMomentLoadRequest request);
}
