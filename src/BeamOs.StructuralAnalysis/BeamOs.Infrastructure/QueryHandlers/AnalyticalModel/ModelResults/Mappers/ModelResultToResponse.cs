using BeamOs.Application.Common.Mappers;
using BeamOs.Contracts.AnalyticalResults.Model;
using BeamOs.Domain.AnalyticalModel.AnalyticalResultsAggregate;
using BeamOs.Domain.Common.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Infrastructure.QueryHandlers.AnalyticalModel.ModelResults.Mappers;

[Mapper]
internal partial class ModelResultToResponse
    : AbstractMapperProvidedUnits<AnalyticalResults, ModelResultResponse>
{
    [Obsolete()]
    public ModelResultToResponse()
        : base(null) { }

    private ModelResultToResponse(UnitSettings unitSettings)
        : base(unitSettings) { }

    public static ModelResultToResponse Create(UnitSettings unitSettings)
    {
        return new(unitSettings);
    }

    public override ModelResultResponse Map(AnalyticalResults source) => this.ToResponse(source);

    public partial ModelResultResponse ToResponse(AnalyticalResults modelResult);
}
