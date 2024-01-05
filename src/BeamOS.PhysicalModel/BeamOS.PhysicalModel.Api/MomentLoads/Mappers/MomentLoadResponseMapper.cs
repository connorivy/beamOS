using BeamOS.Common.Api.Interfaces;
using BeamOS.Common.Api.Mappers;
using BeamOS.PhysicalModel.Contracts.MomentLoad;
using BeamOS.PhysicalModel.Domain.MomentLoadAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Api.MomentLoads.Mappers;

[Mapper]
[UseStaticMapper(typeof(Vector3ToFromMathnetVector))]
public partial class MomentLoadResponseMapper : IMapper<MomentLoad, MomentLoadResponse>
{
    public MomentLoadResponse Map(MomentLoad from) => this.ToResponse(from);

    public partial MomentLoadResponse ToResponse(MomentLoad momentLoad);
}
