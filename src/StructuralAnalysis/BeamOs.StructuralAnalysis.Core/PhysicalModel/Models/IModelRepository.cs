using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.EnvelopeResultSets;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using EntityFramework.Exceptions.Common;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

internal interface IModelRepository : IRepository<ModelId, Model>
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

    public void AddTempModel(Model aggregate);
}

internal sealed class InMemoryModelRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage,
    INodeRepository nodeRepository,
    IInternalNodeRepository internalNodeRepository,
    IElement1dRepository element1dRepository,
    IPointLoadRepository pointLoadRepository,
    IMomentLoadRepository momentLoadRepository,
    IMaterialRepository materialRepository,
    ISectionProfileRepository sectionProfileRepository,
    ISectionProfileFromLibraryRepository sectionProfileFromLibraryRepository,
    ILoadCombinationRepository loadCombinationRepository,
    IResultSetRepository resultSetRepository,
    IEnvelopeResultSetRepository envelopeResultSetRepository,
    IModelProposalRepository modelProposalRepository,
    InMemoryUnitOfWork unitOfWork
) : IModelRepository
{
    public void Add(Model aggregate)
    {
        if (!inMemoryModelRepositoryStorage.Models.TryAdd(aggregate.Id, aggregate))
        {
            unitOfWork.SimulatedFailures.Add(
                new UniqueConstraintException(
                    $"Model with ID {aggregate.Id} already exists in the repository."
                )
            );
        }
    }

    public void AddTempModel(Model aggregate)
    {
        if (!inMemoryModelRepositoryStorage.Models.TryAdd(aggregate.Id, aggregate))
        {
            unitOfWork.SimulatedFailures.Add(
                new UniqueConstraintException(
                    $"Model with ID {aggregate.Id} already exists in the repository."
                )
            );
        }
    }

    public void ClearChangeTracker()
    {
        // No-op for in-memory repository
    }

    public void Attach(Model aggregate)
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
        model.PointLoads = await pointLoadRepository.GetMany(modelId, null, ct);
        model.MomentLoads = await momentLoadRepository.GetMany(modelId, null, ct);
        model.Materials = await materialRepository.GetMany(modelId, null, ct);
        model.SectionProfiles = await sectionProfileRepository.GetMany(modelId, null, ct);
        model.SectionProfilesFromLibrary = await sectionProfileFromLibraryRepository.GetMany(
            modelId,
            null,
            ct
        );
        model.LoadCombinations = await loadCombinationRepository.GetMany(modelId, null, ct);
        model.ResultSets = await resultSetRepository.GetMany(modelId, null, ct);
        model.EnvelopeResultSets = await envelopeResultSetRepository.GetMany(modelId, null, ct);
        model.ModelProposals = await modelProposalRepository.GetMany(modelId, null, ct);

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
