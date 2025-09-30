using BeamOs.Common.Contracts;

namespace BeamOs.StructuralAnalysis.Application.Common;

internal readonly struct ModelEntityCommand : IModelEntity
{
    public Guid ModelId { get; init; }
    public int Id { get; init; }
}
