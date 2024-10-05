using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Contracts.AnalyticalModel.Results;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.AnalyticalResults.ModelResults.Mappers;

//[Mapper]
//public partial class ModelResultToResponseMapper : AbstractMapper<ModelResult, ModelResultResponse>
//{
//    public override ModelResultResponse Map(ModelResult source) => this.ToResponse(source);

//    public partial ModelResultResponse ToResponse(ModelResult source);
//}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
public static partial class ModelResultToResponseMapper
{
    public static partial AnalyticalResultsResponse ToResponse(
        this Domain.AnalyticalModel.AnalyticalResultsAggregate.AnalyticalResults source
    );
}
