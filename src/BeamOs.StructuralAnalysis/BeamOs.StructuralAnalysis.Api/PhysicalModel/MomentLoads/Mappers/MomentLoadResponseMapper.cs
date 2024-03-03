using BeamOs.Api.Common.Interfaces;
using BeamOs.Api.Common.Mappers;
using BeamOS.PhysicalModel.Contracts.MomentLoad;
using BeamOS.PhysicalModel.Domain.MomentLoadAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.MomentLoads.Mappers;

[Mapper]
[UseStaticMapper(typeof(Vector3ToFromMathnetVector))]
public partial class MomentLoadResponseMapper : IMapper<MomentLoad, MomentLoadResponse>
{
    public MomentLoadResponse Map(MomentLoad from) => this.ToResponse(from);

    public partial MomentLoadResponse ToResponse(MomentLoad momentLoad);
}
