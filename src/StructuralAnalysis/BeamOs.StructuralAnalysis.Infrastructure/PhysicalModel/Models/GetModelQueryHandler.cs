using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Models;

public class GetModelQueryHandler(StructuralAnalysisDbContext dbContext)
    : IQueryHandler<Guid, ModelResponse>
{
    public async Task<Result<ModelResponse>> ExecuteAsync(
        Guid query,
        CancellationToken ct = default
    )
    {
        var model = await dbContext
            .Models.AsNoTracking()
            .AsSplitQuery()
            .Where(e => e.Id.Equals(query))
            .Include(m => m.Nodes)
            .Include(m => m.PointLoads)
            .Include(m => m.MomentLoads)
            .Include(m => m.Element1ds)
            .Include(el => el.SectionProfiles)
            .Include(el => el.Materials)
            .Include(m => m.ResultSets)
            .ThenInclude(rs => rs.NodeResults)
            .Include(m => m.LoadCases)
            .Include(m => m.LoadCombinations)
            .FirstOrDefaultAsync(cancellationToken: ct);

        if (model is null)
        {
            return BeamOsError.NotFound();
        }

        var mapper = ModelToResponseMapper.Create(model.Settings.UnitSettings);
        return mapper.Map(model);
    }
}
