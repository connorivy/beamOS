using BeamOs.Common.Contracts;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;

public readonly struct GetModelResourceQuery : IHasModelId
{
    public Guid ModelId { get; init; }
    public int Id { get; init; }
}

public readonly struct EmptyRequest;

public readonly struct GetAnalyticalResultResourceQuery : IHasModelId
{
    public Guid ModelId { get; init; }
    public int ResultSetId { get; init; }
    public int Id { get; init; }
}
