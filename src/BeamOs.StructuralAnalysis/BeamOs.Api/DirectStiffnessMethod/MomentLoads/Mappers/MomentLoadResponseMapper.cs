using BeamOs.Api.Common.Interfaces;
using BeamOs.Api.Common.Mappers;
using BeamOs.Application.DirectStiffnessMethod.MomentLoads;
using BeamOs.Contracts.PhysicalModel.MomentLoad;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Api.MomentLoads.Mappers;

[Mapper]
[UseStaticMapper(typeof(Vector3ToFromMathnetVector))]
[UseStaticMapper(typeof(UnitValueDtoToForceMapper))]
public partial class MomentLoadResponseMapper : IMapper<MomentLoadResponse, CreateMomentLoadCommand>
{
    public CreateMomentLoadCommand Map(MomentLoadResponse source) => this.ToCommand(source);

    [MapProperty(
        nameof(MomentLoadResponse.NormalizedAxisDirection),
        nameof(CreateMomentLoadCommand.AxisDirection)
    )]
    public partial CreateMomentLoadCommand ToCommand(MomentLoadResponse source);
}
