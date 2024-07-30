using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Contracts.AnalyticalResults.Model;
using BeamOs.Domain.AnalyticalResults.ModelResultAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.AnalyticalResults.ModelResults.Mappers;

//[Mapper]
//public partial class ModelResultToResponseMapper : AbstractMapper<ModelResult, ModelResultResponse>
//{
//    public override ModelResultResponse Map(ModelResult source) => this.ToResponse(source);

//    public partial ModelResultResponse ToResponse(ModelResult source);
//}

[Mapper]
[UseStaticMapper(typeof(UnitsNetEnumMapper))]
public static partial class ModelResultToResponseMapper
{
    public static partial ModelResultResponse ToResponse(this ModelResult source);
}
