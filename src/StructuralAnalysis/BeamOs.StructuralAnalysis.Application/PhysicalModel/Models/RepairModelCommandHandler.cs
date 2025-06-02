using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

public class RepairModelCommandHandler(
    IModelRepository modelRepository,
    IModelProposalRepository modelProposalRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceRequest<string>, ModelProposalResponse>
{
    public async Task<Result<ModelProposalResponse>> ExecuteAsync(
        ModelResourceRequest<string> command,
        CancellationToken ct = default
    )
    {
        var model = await modelRepository.GetSingle(
            command.ModelId,
            ct,
            nameof(Model.Nodes),
            nameof(Model.Element1ds)
        );

        if (model is null)
        {
            return BeamOsError.NotFound($"Model with id {command.ModelId} not found.");
        }
        if (model.Nodes is null || model.Nodes.Count == 0)
        {
            return BeamOsError.NotFound($"Model with id {command.ModelId} has no nodes.");
        }
        if (model.Element1ds is null || model.Element1ds.Count == 0)
        {
            return BeamOsError.NotFound($"Model with id {command.ModelId} has no elements.");
        }

        Node firstNodes = model.Nodes[0];
        Octree nodeOctree = new(model.Id, firstNodes.LocationPoint, 10.0);
        foreach (Node node in model.Nodes)
        {
            nodeOctree.Add(node);
        }

        ModelRepairer repairer = new(
            model.Nodes,
            model.Element1ds,
            new(1, LengthUnit.Foot), // hard code 12 inches
            nodeOctree
        );

        ModelProposal proposal = repairer.ProposeRepairs(model);

        modelProposalRepository.Add(proposal);
        await unitOfWork.SaveChangesAsync(ct);

        return proposal.ToContract();
    }
}
