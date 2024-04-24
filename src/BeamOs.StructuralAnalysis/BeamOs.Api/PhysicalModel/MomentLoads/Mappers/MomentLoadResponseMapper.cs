using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Mappers;
using BeamOs.Contracts.PhysicalModel.MomentLoad;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.MomentLoads.Mappers;

[Mapper]
[UseStaticMapper(typeof(Vector3ToFromMathnetVector))]
public partial class MomentLoadResponseMapper : IMapper<MomentLoad, MomentLoadResponse>
{
    public MomentLoadResponse Map(MomentLoad from) => this.ToResponse(from);

    public partial MomentLoadResponse ToResponse(MomentLoad momentLoad);
}
