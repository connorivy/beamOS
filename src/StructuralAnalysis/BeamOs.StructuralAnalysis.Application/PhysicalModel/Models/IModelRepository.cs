using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

public interface IModelRepository : IRepository<ModelId, Model>
{
    public Task<Model?> GetSingle(
        ModelId modelId,
        Func<IQueryable<Model>, IQueryable<Model>>? includeNavigationProperties = null,
        CancellationToken ct = default
    );

    public Task<Model?> GetSingle(
        ModelId modelId,
        CancellationToken ct = default,
        params string[] includeNavigationProperties
    );
}

public sealed class InMemoryModelRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage,
    [FromKeyedServices("InMemory")] INodeRepository nodeRepository,
    [FromKeyedServices("InMemory")] IInternalNodeRepository internalNodeRepository,
    [FromKeyedServices("InMemory")] IElement1dRepository element1dRepository,
    [FromKeyedServices("InMemory")] IMaterialRepository materialRepository,
    [FromKeyedServices("InMemory")] ISectionProfileRepository sectionProfileRepository,
    [FromKeyedServices("InMemory")]
        ISectionProfileFromLibraryRepository sectionProfileFromLibraryRepository
) : IModelRepository
{
    public void Add(Model aggregate)
    {
        inMemoryModelRepositoryStorage.Models.Add(aggregate.Id, aggregate);
    }

    public void ClearChangeTracker()
    {
        // No-op for in-memory repository
    }

    public async Task<Model?> GetSingle(
        ModelId modelId,
        Func<IQueryable<Model>, IQueryable<Model>>? includeNavigationProperties = null,
        CancellationToken ct = default
    )
    {
        var model = inMemoryModelRepositoryStorage.Models.GetValueOrDefault(modelId);
        if (model is null)
        {
            return null;
        }

        model.Nodes = await nodeRepository.GetMany(modelId, null, ct);
        model.InternalNodes = await internalNodeRepository.GetMany(modelId, null, ct);
        model.Element1ds = await element1dRepository.GetMany(
            modelId,
            default(IList<Element1dId>),
            ct
        );
        model.Materials = await materialRepository.GetMany(modelId, null, ct);
        model.SectionProfiles = await sectionProfileRepository.GetMany(modelId, null, ct);
        model.SectionProfilesFromLibrary = await sectionProfileFromLibraryRepository.GetMany(
            modelId,
            null,
            ct
        );

        return model;
    }

    public Task<Model?> GetSingle(
        ModelId modelId,
        CancellationToken ct = default,
        params string[] includeNavigationProperties
    ) => this.GetSingle(modelId, null, ct);

    public ValueTask Put(Model aggregate)
    {
        if (inMemoryModelRepositoryStorage.Models.ContainsKey(aggregate.Id))
        {
            inMemoryModelRepositoryStorage.Models[aggregate.Id] = aggregate;
            return ValueTask.CompletedTask;
        }
        else
        {
            throw new KeyNotFoundException($"Model with ID {aggregate.Id} not found.");
        }
    }

    public void Remove(Model aggregate)
    {
        inMemoryModelRepositoryStorage.Models.Remove(aggregate.Id);
    }
}

public class InMemoryGetModelsQueryHandler(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
) : IQueryHandler<EmptyRequest, ICollection<ModelInfoResponse>>
{
    public Task<Result<ICollection<ModelInfoResponse>>> ExecuteAsync(
        EmptyRequest query,
        CancellationToken ct = default
    )
    {
        var models = inMemoryModelRepositoryStorage.Models.Values;

        Result<ICollection<ModelInfoResponse>> result = models
            .Select(m => new ModelInfoResponse(
                m.Id,
                m.Name,
                m.Description,
                m.Settings.ToContract(),
                m.LastModified,
                "Owner"
            ))
            .ToList();
        return Task.FromResult(result);
    }
}

public sealed class InMemoryRestoreModeCommandHandler()
    : ICommandHandler<ModelResourceRequest<DateTimeOffset>, ModelResponse>
{
    public Task<Result<ModelResponse>> ExecuteAsync(
        ModelResourceRequest<DateTimeOffset> command,
        CancellationToken ct = default
    )
    {
        throw new NotImplementedException("InMemoryRestoreModeCommandHandler is not implemented.");
    }
}
