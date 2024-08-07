using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Application.PhysicalModel.Element1dAggregate;
using BeamOs.Contracts.PhysicalModel.Element1d;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Element1ds.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
public static partial class CreateElement1DCommandMapper
{
    public static partial CreateElement1dCommand ToCommand(this CreateElement1dRequest request);
}
