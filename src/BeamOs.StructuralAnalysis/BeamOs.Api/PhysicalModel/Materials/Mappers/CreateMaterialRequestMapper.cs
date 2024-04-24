using BeamOs.Api.Common.Mappers;
using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.PhysicalModel.Materials;
using BeamOs.Contracts.PhysicalModel.Material;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Materials.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitValueDtoToPressureMapper))]
public partial class CreateMaterialRequestMapper
    : IMapper<CreateMaterialRequest, CreateMaterialCommand>
{
    public CreateMaterialCommand Map(CreateMaterialRequest from) => this.ToCommand(from);

    public partial CreateMaterialCommand ToCommand(CreateMaterialRequest request);
}
