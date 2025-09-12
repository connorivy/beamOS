
namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;

internal interface IGetAnalyticalResultResourceQuery
{
    int Id { get; init; }
    Guid ModelId { get; init; }
    int ResultSetId { get; init; }
}
