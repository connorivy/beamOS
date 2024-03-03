using BeamOs.Api.Common.Interfaces;
using BeamOs.Api.Common.Mappers;
using BeamOS.PhysicalModel.Application.Materials;
using BeamOS.PhysicalModel.Contracts.Material;
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
