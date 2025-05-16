using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;

public class PatchNodeCommandHandler(
    INodeRepository nodeRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<PatchNodeCommand, NodeResponse>
{
    public async Task<Result<NodeResponse>> ExecuteAsync(
        PatchNodeCommand command,
        CancellationToken ct = default
    )
    {
        var node = await nodeRepository.Update(command);
        await unitOfWork.SaveChangesAsync(ct);

        return node.ToResponse();
    }
}

public readonly struct PatchNodeCommand : IModelResourceRequest<UpdateNodeRequest>
{
    public Guid ModelId { get; init; }
    public UpdateNodeRequest Body { get; init; }
    public int Id => this.Body.Id;
    public PartialPoint? LocationPoint => this.Body.LocationPoint;
    public PartialRestraint? Restraint => this.Body.Restraint;
}
