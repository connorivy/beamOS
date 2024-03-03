using BeamOs.Api.Common.Interfaces;
using BeamOs.Api.Common.Mappers;
using BeamOS.PhysicalModel.Contracts.PointLoad;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.PointLoads.Mappers;

[Mapper]
[UseStaticMapper(typeof(Vector3ToFromMathnetVector))]
public partial class PointLoadResponseMapper : IMapper<PointLoad, PointLoadResponse>
{
    public PointLoadResponse Map(PointLoad from) => this.ToResponse(from);

    public partial PointLoadResponse ToResponse(PointLoad pointLoad);
}
