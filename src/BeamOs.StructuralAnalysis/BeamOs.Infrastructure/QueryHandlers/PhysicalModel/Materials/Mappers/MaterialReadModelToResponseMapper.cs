using BeamOs.Application.Common.Mappers;
using BeamOs.Contracts.PhysicalModel.Material;
using BeamOs.Infrastructure.Data.Models;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Materials.Mappers;

[Mapper]
internal partial class MaterialReadModelToResponseMapper
    : AbstractMapper<MaterialReadModel, MaterialResponse>
{
    public override MaterialResponse Map(MaterialReadModel source) => this.ToResponse(source);

    public partial MaterialResponse ToResponse(MaterialReadModel source);
}
