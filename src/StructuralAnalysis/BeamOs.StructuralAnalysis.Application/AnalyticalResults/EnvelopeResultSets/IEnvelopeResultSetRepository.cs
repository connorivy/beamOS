using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.EnvelopeResultSets;

namespace BeamOs.StructuralAnalysis.Application.AnalyticalResults.EnvelopeResultSets;

public interface IEnvelopeResultSetRepository
    : IModelResourceRepository<EnvelopeResultSetId, EnvelopeResultSet> { }

public sealed class InMemoryEnvelopeResultSetRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<EnvelopeResultSetId, EnvelopeResultSet>(
        inMemoryModelRepositoryStorage
    ),
        IEnvelopeResultSetRepository { }
