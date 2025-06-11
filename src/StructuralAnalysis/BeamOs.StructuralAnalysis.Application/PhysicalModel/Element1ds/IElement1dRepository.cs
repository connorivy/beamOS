using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;

public interface IElement1dRepository : IModelResourceRepository<Element1dId, Element1d>
{
    //public Task<List<Element1D>> GetElement1dsInModel(
    //    ModelId modelId,
    //    CancellationToken ct = default
    //);
    public Task<List<Element1d>> GetMany(
        ModelId modelId,
        IList<NodeId> startOrEndNodeIds,
        CancellationToken ct = default
    );
}

public interface IElement1dProposalRepository
    : IProposalRepository<Element1dProposalId, Element1dProposal> { }

public sealed class InMemoryElement1dRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<Element1dId, Element1d>(inMemoryModelRepositoryStorage),
        IElement1dRepository
{
    public Task<List<Element1d>> GetMany(
        ModelId modelId,
        IList<NodeId> startOrEndNodeIds,
        CancellationToken ct = default
    )
    {
        if (!this.ModelResources.TryGetValue(modelId, out var resources))
        {
            return Task.FromResult(new List<Element1d>());
        }
        var nodeIdHashSet = new HashSet<NodeId>(startOrEndNodeIds);

        return Task.FromResult(
            resources
                .Values.Where(e =>
                    nodeIdHashSet.Contains(e.StartNodeId) || nodeIdHashSet.Contains(e.EndNodeId)
                )
                .ToList()
        );
    }
}
