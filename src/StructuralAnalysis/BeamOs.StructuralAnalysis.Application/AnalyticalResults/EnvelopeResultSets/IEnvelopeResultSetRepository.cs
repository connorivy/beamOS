using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.EnvelopeResultSets;

namespace BeamOs.StructuralAnalysis.Application.AnalyticalResults.EnvelopeResultSets;

public interface IEnvelopeResultSetRepository
    : IModelResourceRepository<EnvelopeResultSetId, EnvelopeResultSet> { }
