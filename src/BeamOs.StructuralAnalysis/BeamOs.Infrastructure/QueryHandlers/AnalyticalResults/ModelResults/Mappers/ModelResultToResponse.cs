using BeamOs.Application.Common.Mappers;
using BeamOs.Contracts.AnalyticalResults.Model;
using BeamOs.Domain.AnalyticalResults.ModelResultAggregate;
using BeamOs.Domain.Common.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Infrastructure.QueryHandlers.AnalyticalResults.ModelResults.Mappers;

[Mapper]
internal partial class ModelResultToResponse
    : AbstractMapperProvidedUnits<ModelResult, ModelResultResponse>
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

    public override ModelResultResponse Map(ModelResult source) => this.ToResponse(source);

    public partial ModelResultResponse ToResponse(ModelResult modelResult);
}
