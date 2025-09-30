using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;

internal interface IElement1dRepository : IModelResourceRepository<Element1dId, Element1d>
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

internal interface IElement1dProposalRepository
    : IProposalRepository<Element1dProposalId, Element1dProposal> { }

internal sealed class InMemoryElement1dRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage,
    ISectionProfileRepository sectionProfileRepository,
    ISectionProfileFromLibraryRepository sectionProfileFromLibraryRepository,
    INodeRepository nodeRepository,
    IInternalNodeRepository internalNodeRepository,
    IMaterialRepository materialRepository
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

    public override async Task<List<Element1d>> GetMany(
        ModelId modelId,
        IList<Element1dId>? ids,
        CancellationToken ct = default
    )
    {
        var elements = await base.GetMany(modelId, ids, ct);
        foreach (var el in elements)
        {
            el.SectionProfile = await sectionProfileRepository.GetSingle(
                modelId,
                el.SectionProfileId,
                ct
            );
            el.SectionProfile ??= await sectionProfileFromLibraryRepository.GetSingle(
                modelId,
                el.SectionProfileId,
                ct
            );
            el.StartNode = await nodeRepository.GetSingle(modelId, el.StartNodeId, ct);
            el.EndNode = await nodeRepository.GetSingle(modelId, el.EndNodeId, ct);
            el.InternalNodes =
            [
                .. (await internalNodeRepository.GetMany(modelId, null, ct)).Where(n =>
                    n.Element1dId == el.Id
                ),
            ];
            foreach (var internalNode in el.InternalNodes)
            {
                internalNode.Element1d = el;
            }
            el.Material = await materialRepository.GetSingle(modelId, el.MaterialId, ct);
        }

        return elements;
    }
}
