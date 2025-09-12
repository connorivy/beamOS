using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Models;

internal class GetModelInfoQueryHandler(StructuralAnalysisDbContext dbContext)
    : IQueryHandler<Guid, ModelInfoResponse>
{
    public async Task<Result<ModelInfoResponse>> ExecuteAsync(
        Guid query,
        CancellationToken ct = default
    )
    {
        var model = await dbContext
            .Models.AsNoTracking()
            .Where(e => e.Id.Equals(query))
            .FirstOrDefaultAsync(cancellationToken: ct);

        if (model is null)
        {
            return BeamOsError.NotFound();
        }

        return new ModelInfoResponse(
            model.Id,
            model.Name,
            model.Description,
            model.Settings.ToContract(),
            model.LastModified,
            "Owner"
        );
    }
}
