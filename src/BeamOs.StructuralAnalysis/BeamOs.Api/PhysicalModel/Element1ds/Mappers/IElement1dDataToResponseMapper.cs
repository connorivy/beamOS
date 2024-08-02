using BeamOs.Application.Common.Mappers;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Application.PhysicalModel.Element1dAggregate.Interfaces;
using BeamOs.Contracts.PhysicalModel.Element1d;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Element1ds.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
public partial class IElement1dDataToResponseMapper
    : AbstractMapper<IElement1dData, Element1dResponseHydrated>
{
    public override Element1dResponseHydrated Map(IElement1dData source) => this.ToResponse(source);

    public partial Element1dResponseHydrated ToResponse(IElement1dData source);
}
