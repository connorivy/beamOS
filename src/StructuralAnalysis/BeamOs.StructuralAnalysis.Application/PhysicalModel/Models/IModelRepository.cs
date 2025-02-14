using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

public interface IModelRepository
{
    void Add(Model entity);

    //Task<Model> Update(PatchModelCommand patchCommand);
    //Task<List<Model>> GetAll();
    Task<Model?> GetSingle(
        ModelId modelId,
        Func<IQueryable<Model>, IQueryable<Model>>? includeNavigationProperties = null,
        CancellationToken ct = default
    );

    Task<Model?> GetSingle(
        ModelId modelId,
        //Func<IQueryable<Model>, IQueryable<Model>>? includeNavigationProperties = null,
        CancellationToken ct = default,
        params string[] includeNavigationProperties
    );
}
