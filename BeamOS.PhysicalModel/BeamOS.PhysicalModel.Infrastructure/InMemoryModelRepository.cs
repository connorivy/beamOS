using BeamOS.PhysicalModel.Application.Common.Interfaces;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;

namespace BeamOS.PhysicalModel.Infrastructure;

public class InMemoryModelRepository : IRepository<ModelId, Model>
{
    private readonly Dictionary<ModelId, Model> models = [];
    public Task Add(Model model)
    {
        _ = this.models.TryAdd(model.Id, model);
        return Task.CompletedTask;
    }
    public async Task<Model?> GetById(ModelId modelId)
    {
        await Task.CompletedTask;

        _ = this.models.TryGetValue(modelId, out Model? result);
        return result;
    }
}
