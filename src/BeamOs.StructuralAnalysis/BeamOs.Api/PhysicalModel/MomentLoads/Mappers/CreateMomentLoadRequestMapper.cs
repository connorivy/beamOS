using BeamOs.Api.Common.Mappers;
using BeamOs.Application.Common.Mappers;
using BeamOs.Application.PhysicalModel.MomentLoads;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.PhysicalModel.MomentLoad;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.MomentLoads.Mappers;

[Mapper]
[UseStaticMapper(typeof(Vector3ToFromMathnetVector))]
[UseStaticMapper(typeof(UnitValueDtoToTorqueMapper))]
public partial class CreateMomentLoadRequestMapper
    : IMapper<CreateMomentLoadRequest, CreateMomentLoadCommand>
{
    public CreateMomentLoadCommand Map(CreateMomentLoadRequest from) => this.ToCommand(from);

    public partial CreateMomentLoadCommand ToCommand(CreateMomentLoadRequest request);
}
