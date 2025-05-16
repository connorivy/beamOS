using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

public class CreateProposalCommandHandler(
    IModelRepository modelRepository,
    INodeRepository nodeRepository,
    IElement1dRepository element1dRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceRequest<ModelData>, ModelResponse>
{
    public async Task<Result<ModelResponse>> ExecuteAsync(
        ModelResourceRequest<ModelData> command,
        CancellationToken ct = default
    )
    {
        // foreach (var node in command.Data.Nodes)
        // {
        //     var nodeResult = await nodeRepository.PutAsync(node, ct);
        //     if (nodeResult.IsFailure)
        //         return nodeResult.ConvertFailure<ModelResponse>();
        // }
        return default;
    }
}
