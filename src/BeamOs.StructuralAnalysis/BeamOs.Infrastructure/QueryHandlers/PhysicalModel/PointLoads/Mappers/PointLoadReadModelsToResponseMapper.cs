using BeamOs.Application.Common.Mappers;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOs.Infrastructure.Data.Models;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.PointLoads.Mappers;

[Mapper]
internal partial class PointLoadReadModelsToResponseMapper
    : AbstractMapper<PointLoadReadModel, PointLoadResponse>
{
    public override PointLoadResponse Map(PointLoadReadModel source) => this.ToResponse(source);

    public partial PointLoadResponse ToResponse(PointLoadReadModel source);
}
