using BeamOS.PhysicalModel.Contracts;
using BeamOS.PhysicalModel.Domain.AnalyticalModelAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Api.Mappers;
[Mapper]
public static partial class ModelResponseMapper
{
    public static partial ModelResponse ToResponse(this AnalyticalModel model);
}
