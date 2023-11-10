using BeamOS.PhysicalModel.Contracts.PointLoad;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Api.PointLoads.Mappers;
[Mapper]
[UseStaticMapper(typeof(Vector3Mapper))]
public static partial class PointLoadResponseMapper
{
    public static partial PointLoadResponse ToResponse(this PointLoad pointLoad);
}
