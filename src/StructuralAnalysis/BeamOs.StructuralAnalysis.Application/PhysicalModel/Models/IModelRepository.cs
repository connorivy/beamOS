using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

public interface IModelRepository
{
    void Add(Model entity);
    //Task<Model> Update(PatchModelCommand patchCommand);
    //Task<List<Model>> GetAll();
}
