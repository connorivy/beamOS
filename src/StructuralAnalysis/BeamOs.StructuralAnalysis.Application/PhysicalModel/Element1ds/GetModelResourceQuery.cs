using BeamOs.Common.Application;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;

public readonly struct GetModelResourceQuery : IHasModelId
{
    public Guid ModelId { get; init; }
    public int Id { get; init; }
}

public readonly struct GetAnalyticalResultResourceQuery : IHasModelId
{
    public Guid ModelId { get; init; }
    public int ResultSetId { get; init; }
    public int Id { get; init; }
}
