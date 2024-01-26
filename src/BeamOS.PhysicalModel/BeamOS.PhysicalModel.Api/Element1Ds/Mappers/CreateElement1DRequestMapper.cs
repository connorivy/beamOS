using BeamOS.Common.Api.Mappers;
using BeamOS.PhysicalModel.Application.Element1Ds;
using BeamOS.PhysicalModel.Contracts.Element1D;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Api.Element1Ds.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitValueDtoToAngleMapper))]
public static partial class CreateElement1DCommandMapper
{
    public static partial CreateElement1DCommand ToCommand(this CreateElement1DRequest request);
}
