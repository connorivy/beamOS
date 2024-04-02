using BeamOs.Api.Common.Interfaces;
using BeamOs.Api.Common.Mappers;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.PointLoads.Mappers;

[Mapper]
[UseStaticMapper(typeof(Vector3ToFromMathnetVector))]
public partial class PointLoadResponseMapper : IMapper<PointLoad, PointLoadResponse>
{
    public PointLoadResponse Map(PointLoad from) => this.ToResponse(from);

    [MapProperty(nameof(@PointLoad.Data.Force), nameof(PointLoadResponse.Force))]
    [MapProperty(
        nameof(@PointLoad.Data.NormalizedDirection),
        nameof(PointLoadResponse.NormalizedDirection)
    )]
    public partial PointLoadResponse ToResponse(PointLoad pointLoad);
}
