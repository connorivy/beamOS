using BeamOs.Application.Common.Mappers;
using BeamOs.Application.PhysicalModel.Element1dAggregate.Interfaces;
using BeamOs.Contracts.PhysicalModel.Element1d;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Element1d.Mappers;

[Mapper]
internal partial class Element1dReadModelToHydratedResponseMapper(
    NodeReadModelToResponseMapper nodeReadModelToHydratedResponseMapper
) : AbstractMapper<IElement1dData, Element1dResponseHydrated>
{
    [UseMapper]
    private readonly NodeReadModelToResponseMapper nodeResponseMapper =
        nodeReadModelToHydratedResponseMapper;

    public override Element1dResponseHydrated Map(IElement1dData source) => this.ToResponse(source);

    private partial Element1dResponseHydrated ToResponse(IElement1dData source);
}
