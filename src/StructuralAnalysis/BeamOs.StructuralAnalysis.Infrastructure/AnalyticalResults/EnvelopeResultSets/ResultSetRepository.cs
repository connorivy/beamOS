using BeamOs.StructuralAnalysis.Application.AnalyticalResults.EnvelopeResultSets;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.EnvelopeResultSets;
using BeamOs.StructuralAnalysis.Infrastructure.Common;

namespace BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.EnvelopeResultSets;

internal sealed class EnvelopeResultSetRepository(StructuralAnalysisDbContext dbContext)
    : ModelResourceRepositoryBase<EnvelopeResultSetId, EnvelopeResultSet>(dbContext),
        IEnvelopeResultSetRepository { }
