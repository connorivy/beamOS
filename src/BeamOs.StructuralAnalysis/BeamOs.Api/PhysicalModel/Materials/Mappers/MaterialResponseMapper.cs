using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.PhysicalModel.Material;
using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Materials.Mappers;

[Mapper]
public partial class MaterialResponseMapper : IMapper<Material, MaterialResponse>
{
    public MaterialResponse Map(Material from) => this.ToResponse(from);

    public partial MaterialResponse ToResponse(Material model);
}
