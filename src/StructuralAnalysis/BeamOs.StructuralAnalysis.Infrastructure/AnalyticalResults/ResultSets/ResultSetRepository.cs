using BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.ResultSets;

internal sealed class ResultSetRepository(StructuralAnalysisDbContext dbContext)
    : ModelResourceRepositoryBase<ResultSetId, ResultSet>(dbContext),
        IResultSetRepository
{
    public Task<int> DeleteAll(ModelId modelId, CancellationToken ct) =>
        this.DbContext.ResultSets.Where(s => s.ModelId == modelId).ExecuteDeleteAsync(ct);

    public async Task<ResultSet?> GetSingle(
        ModelId modelId,
        ResultSetId resultSetId,
        CancellationToken ct,
        params string[] resultSetMembersToLoad
    )
    {
        var query = this
            .DbContext.ResultSets.AsNoTracking()
            .AsSplitQuery()
            .Where(s => s.ModelId == modelId && s.Id == resultSetId);

        foreach (var member in resultSetMembersToLoad)
        {
            query = query.Include(member);
        }

        return await query.FirstOrDefaultAsync(ct);
    }

    public override async Task<ModelSettingsAndEntity<ResultSet>?> GetSingleWithModelSettings(
        ModelId modelId,
        ResultSetId id,
        CancellationToken ct = default
    )
    {
        var settingAndEntity = await this
            .DbContext.ResultSets.AsNoTracking()
            .AsSplitQuery()
            .Include(el => el.NodeResults)
            .Include(el => el.Element1dResults)
            .Where(m => m.ModelId == modelId && m.Id.Equals(id))
            .Select(m => new { ModelSettings = m.Model.Settings, Entity = m })
            .FirstOrDefaultAsync(ct);

        return settingAndEntity is null
            ? null
            : new ModelSettingsAndEntity<ResultSet>(
                settingAndEntity.ModelSettings,
                settingAndEntity.Entity
            );
    }
}
