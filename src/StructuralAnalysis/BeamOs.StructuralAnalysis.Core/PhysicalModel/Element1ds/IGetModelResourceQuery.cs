namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;

internal interface IGetModelResourceQuery
{
    int Id { get; init; }
    Guid ModelId { get; init; }
}
