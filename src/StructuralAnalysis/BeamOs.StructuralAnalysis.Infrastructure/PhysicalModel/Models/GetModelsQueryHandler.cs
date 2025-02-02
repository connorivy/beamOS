using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Models;

public class GetModelsQueryHandler(StructuralAnalysisDbContext dbContext)
    : IQueryHandler<EmptyRequest, List<ModelInfoResponse>>
{
    public async Task<Result<List<ModelInfoResponse>>> ExecuteAsync(
        EmptyRequest query,
        CancellationToken ct = default
    )
    {
        return await dbContext
            .Models
            .Select(
                m => new ModelInfoResponse(m.Id, m.Name, m.Description, m.Settings.ToContract())
            )
            .ToListAsync(cancellationToken: ct);
    }
}
