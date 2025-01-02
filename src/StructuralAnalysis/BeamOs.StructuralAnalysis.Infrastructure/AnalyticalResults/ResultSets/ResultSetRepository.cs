using BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Infrastructure.Common;

namespace BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.ResultSets;

internal sealed class ResultSetRepository(StructuralAnalysisDbContext dbContext)
    : ModelResourceRepositoryBase<ResultSetId, ResultSet>(dbContext),
        IResultSetRepository { }
