using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;

namespace BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;

public interface IResultSetRepository : IModelResourceRepository<ResultSetId, ResultSet> { }
