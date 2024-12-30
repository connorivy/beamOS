namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;

public readonly struct GetModelResourceQuery
{
    public Guid ModelId { get; init; }
    public int Id { get; init; }
}
