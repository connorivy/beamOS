using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;

public interface IElement1dRepository : IModelResourceRepository<Element1dId, Element1d>
{
    //public Task<List<Element1D>> GetElement1dsInModel(
    //    ModelId modelId,
    //    CancellationToken ct = default
    //);
}

public interface IElement1dProposalRepository
    : IProposalRepository<Element1dProposalId, Element1dProposal> { }

public sealed class InMemoryElement1dRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<Element1dId, Element1d>(inMemoryModelRepositoryStorage),
        IElement1dRepository { }
