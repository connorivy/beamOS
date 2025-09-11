
namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;

public interface IGetModelResourceQuery
{
    int Id { get; init; }
    Guid ModelId { get; init; }
}