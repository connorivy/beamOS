using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Application.PhysicalModel.Materials;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.PhysicalModel.Material;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Materials.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
public partial class CreateMaterialRequestMapper
    : IMapper<CreateMaterialRequest, CreateMaterialCommand>
{
    public CreateMaterialCommand Map(CreateMaterialRequest from) => this.ToCommand(from);

    public partial CreateMaterialCommand ToCommand(CreateMaterialRequest request);
}
