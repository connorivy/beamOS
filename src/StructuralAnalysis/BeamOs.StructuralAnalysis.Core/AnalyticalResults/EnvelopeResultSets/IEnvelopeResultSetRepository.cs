using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.EnvelopeResultSets;

namespace BeamOs.StructuralAnalysis.Application.AnalyticalResults.EnvelopeResultSets;

internal interface IEnvelopeResultSetRepository
    : IModelResourceRepository<EnvelopeResultSetId, EnvelopeResultSet> { }

internal sealed class InMemoryEnvelopeResultSetRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<EnvelopeResultSetId, EnvelopeResultSet>(
        inMemoryModelRepositoryStorage
    ),
        IEnvelopeResultSetRepository { }
