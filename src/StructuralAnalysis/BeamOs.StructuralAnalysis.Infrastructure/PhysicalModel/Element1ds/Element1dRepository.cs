using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Element1ds;

internal class Element1dRepository(StructuralAnalysisDbContext dbContext)
    : ModelResourceRepositoryBase<Element1dId, Element1d>(dbContext),
        IElement1dRepository
{
    public Task<List<Element1d>> GetMany(
        ModelId modelId,
        IList<NodeId> startOrEndNodeIds,
        CancellationToken ct = default
    )
    {
        return this
            .DbContext.Element1ds.Where(e =>
                e.ModelId == modelId
                && (
                    startOrEndNodeIds.Contains(e.StartNodeId)
                    || startOrEndNodeIds.Contains(e.EndNodeId)
                )
            )
            .ToListAsync(ct);
    }
}
