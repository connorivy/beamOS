using BeamOs.Api.Common.Mappers;
using BeamOs.Application.Common.Models;
using BeamOs.Application.DirectStiffnessMethod.Materials;
using BeamOs.Contracts.PhysicalModel.Material;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Api.Materials.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitValueDtoToPressureMapper))]
public partial class MaterialResponseToCreateMaterialCommand
    : AbstractMapper<MaterialResponse, CreateMaterialCommand>
{
    public override CreateMaterialCommand Map(MaterialResponse source) => this.ToCommand(source);

    public partial CreateMaterialCommand ToCommand(MaterialResponse source);
}
