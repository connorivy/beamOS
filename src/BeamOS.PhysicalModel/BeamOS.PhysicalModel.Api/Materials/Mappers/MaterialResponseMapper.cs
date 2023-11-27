using BeamOS.Common.Api.Interfaces;
using BeamOS.PhysicalModel.Contracts.Material;
using BeamOS.PhysicalModel.Domain.MaterialAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Api.Mappers;

[Mapper]
public partial class MaterialResponseMapper : IMapper<Material, MaterialResponse>
{
    public MaterialResponse Map(Material from) => this.ToResponse(from);
    public partial MaterialResponse ToResponse(Material model);
}
