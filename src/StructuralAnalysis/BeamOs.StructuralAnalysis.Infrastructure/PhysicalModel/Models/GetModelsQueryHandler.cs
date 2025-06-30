using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Tests.Common;
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
        var sampleModels = AllSolvedProblems
            .ModelFixtures()
            .Select(m => new ModelId(m.Id))
            .ToHashSet();

        return await dbContext
            .Models.Where(m => !sampleModels.Contains(m.Id))
            .Select(m => new ModelInfoResponse(
                m.Id,
                m.Name,
                m.Description,
                m.Settings.ToContract(),
                m.LastModified,
                "Owner"
            ))
            .ToListAsync(cancellationToken: ct);
    }
}
