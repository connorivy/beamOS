using BeamOS.Common.Api.Interfaces;
using BeamOS.PhysicalModel.Contracts.PointLoad;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Api.PointLoads.Mappers;
[Mapper]
[UseStaticMapper(typeof(Vector3Mapper))]
public partial class PointLoadResponseMapper : IMapper<PointLoad, PointLoadResponse>
{
    public PointLoadResponse Map(PointLoad from) => this.ToResponse(from);
    public partial PointLoadResponse ToResponse(PointLoad pointLoad);
}
