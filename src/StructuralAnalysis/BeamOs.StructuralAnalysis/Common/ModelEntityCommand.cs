using BeamOs.Common.Contracts;

namespace BeamOs.StructuralAnalysis.Application.Common;

public readonly struct ModelEntityCommand : IModelEntity
{
    public Guid ModelId { get; init; }
    public int Id { get; init; }
}
