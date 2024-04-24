using BeamOs.Application.Common.Mappers;
using BeamOs.Application.Common.Models;
using BeamOs.Contracts.PhysicalModel.MomentLoad;
using BeamOs.Infrastructure.Data.Models;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.MomentLoads.Mapper;

[Mapper]
[UseStaticMapper(typeof(Vector3ToFromMathnetVector))]
internal partial class MomentLoadReadModelToResponseMapper
    : AbstractMapper<MomentLoadReadModel, MomentLoadResponse>
{
    public override MomentLoadResponse Map(MomentLoadReadModel source) => this.ToResponse(source);

    public partial MomentLoadResponse ToResponse(MomentLoadReadModel source);
}
