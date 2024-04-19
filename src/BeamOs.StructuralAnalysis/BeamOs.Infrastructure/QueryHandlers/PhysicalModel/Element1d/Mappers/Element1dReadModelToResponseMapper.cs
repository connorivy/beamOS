using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Models;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Infrastructure.Data.Models;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Element1d.Mappers;

[Mapper]
internal partial class Element1dReadModelToResponseMapper(
    NodeReadModelToResponseMapper nodeReadModelToHydratedResponseMapper
) : AbstractMapper<Element1dReadModel, Element1dResponseHydrated>
{
    [UseMapper]
    private readonly NodeReadModelToResponseMapper nodeResponseMapper =
        nodeReadModelToHydratedResponseMapper;

    public override Element1dResponseHydrated Map(Element1dReadModel source) =>
        this.ToResponse(source);

    public partial Element1dResponseHydrated ToResponse(Element1dReadModel source);
}
