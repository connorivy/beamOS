using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Models;

internal class GetModelsQueryHandler(StructuralAnalysisDbContext dbContext)
    : IQueryHandler<EmptyRequest, ICollection<ModelInfoResponse>>
{
    public async Task<Result<ICollection<ModelInfoResponse>>> ExecuteAsync(
        EmptyRequest query,
        CancellationToken ct = default
    )
    {
        Guid[] sampleModelIds =
        [
            Guid.Parse("4ce66084-4ac1-40bc-99ae-3d0f334c66fa"), // twisty bowl framing
            Guid.Parse("0a83df88-656e-47d9-98fe-25fd7d370b06"), // Kassimali_Example3_8
            Guid.Parse("6b04df0f-45d6-4aed-9c04-8272ed23f811") // Kassimali_Example8_4
        ];

        return await dbContext
            .Models.Where(m => !sampleModelIds.Contains(m.Id))
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
