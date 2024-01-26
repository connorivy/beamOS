using BeamOS.Common.Api.Interfaces;
using BeamOS.Common.Api.Mappers;
using BeamOS.PhysicalModel.Application.Materials;
using BeamOS.PhysicalModel.Contracts.Material;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Api.Materials.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitValueDtoToPressureMapper))]
public partial class CreateMaterialRequestMapper
    : IMapper<CreateMaterialRequest, CreateMaterialCommand>
{
    public CreateMaterialCommand Map(CreateMaterialRequest from) => this.ToCommand(from);

    public partial CreateMaterialCommand ToCommand(CreateMaterialRequest request);
}
