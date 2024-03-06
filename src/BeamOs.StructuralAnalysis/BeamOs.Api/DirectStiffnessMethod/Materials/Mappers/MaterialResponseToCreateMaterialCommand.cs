using BeamOS.Common.Api;
using BeamOS.Common.Api.Mappers;
using BeamOS.DirectStiffnessMethod.Application.Materials;
using BeamOS.PhysicalModel.Contracts.Material;
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
