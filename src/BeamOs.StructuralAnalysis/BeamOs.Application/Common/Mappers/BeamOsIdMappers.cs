using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;

namespace BeamOs.Application.Common.Mappers;

public static class BeamOsIdMappers
{
    public static ModelId ToId(string modelId) => new(Guid.Parse(modelId));
}
