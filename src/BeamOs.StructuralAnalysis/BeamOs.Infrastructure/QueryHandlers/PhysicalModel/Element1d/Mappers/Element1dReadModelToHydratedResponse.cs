using BeamOs.Application.Common.Interfaces;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Infrastructure.Data.Models;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Element1d.Mappers;

[Mapper]
internal partial class Element1dReadModelToHydratedResponse(
    NodeReadModelToHydratedResponseMapper nodeReadModelToHydratedResponseMapper
) : IMapper<Element1dReadModel, Element1dResponseHydrated>
{
    [UseMapper]
    private readonly NodeReadModelToHydratedResponseMapper nodeResponseMapper =
        nodeReadModelToHydratedResponseMapper;

    public Element1dResponseHydrated Map(Element1dReadModel source) =>
        this.ToHydratedResponse(source);

    public partial Element1dResponseHydrated ToHydratedResponse(Element1dReadModel source);
}
