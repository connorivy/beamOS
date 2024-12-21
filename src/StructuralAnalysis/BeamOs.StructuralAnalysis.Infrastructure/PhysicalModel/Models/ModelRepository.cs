using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Models;

internal class ModelRepository(StructuralAnalysisDbContext dbContext) : IModelRepository
{
    public void Add(Model entity) => dbContext.Models.Add(entity);
}
