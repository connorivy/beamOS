
namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;

public interface IGetAnalyticalResultResourceQuery
{
    int Id { get; init; }
    Guid ModelId { get; init; }
    int ResultSetId { get; init; }
}