using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

public class AcceptProposalCommandHandler(
    IModelProposalRepository modelProposalRepository,
    INodeRepository nodeRepository,
    IElement1dRepository element1dRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<IModelEntity, ModelResponse>
{
    public async Task<Result<ModelResponse>> ExecuteAsync(
        IModelEntity command,
        CancellationToken ct = default
    )
    {
        var modelProposal = await modelProposalRepository.GetSingle(
            command.ModelId,
            command.Id,
            ct
        );
        if (modelProposal is null)
        {
            return BeamOsError.NotFound(
                description: $"Model proposal with id {command.Id} for model {command.ModelId} was not found"
            );
        }

        Dictionary<NodeProposalId, NodeId> nodeProposalIdToNewIdDict = [];
        List<Node> nodes = [];
        foreach (var nodeProposal in modelProposal.NodeProposals ?? [])
        {
            var node = nodeProposal.ToDomain();
            nodes.Add(node);
            if (nodeProposal.IsExisting)
            {
                nodeRepository.Put(node);
            }
            else
            {
                nodeRepository.Add(node);
                nodeProposalIdToNewIdDict[nodeProposal.Id] = node.Id;
            }
        }

        Dictionary<Element1dProposalId, Element1dId> element1dProposalIdToNewIdDict = [];
        List<Element1d> element1ds = [];
        foreach (var element1dProposal in modelProposal.Element1dProposals ?? [])
        {
            var element1d = element1dProposal.ToDomain(nodeProposalIdToNewIdDict);
            element1ds.Add(element1d);
            if (element1dProposal.IsExisting)
            {
                element1dRepository.Put(element1d);
            }
            else
            {
                element1dRepository.Add(element1d);
                element1dProposalIdToNewIdDict[element1dProposal.Id] = element1d.Id;
            }
        }
        modelProposalRepository.Remove(modelProposal);

        await unitOfWork.SaveChangesAsync(ct);

        ModelToResponseMapper modelToResponseMapper = ModelToResponseMapper.Create(
            modelProposal.Settings.UnitSettings
        );
        ModelResponse response = new(
            modelProposal.ModelId,
            modelProposal.Name,
            modelProposal.Description,
            modelProposal.Settings.ToContract(),
            DateTimeOffset.UtcNow,
            [.. nodes.Select(n => n.ToResponse())],
            [.. element1ds.Select(e => e.ToResponse())]
        // modelProposal.Materials?.Select(m => m.ToContract()).ToList(),
        // modelProposal.SectionProfiles?.Select(s => s.ToContract()).ToList(),
        // modelProposal.PointLoads?.Select(p => p.ToContract()).ToList(),
        // modelProposal.MomentLoads?.Select(m => m.ToContract()).ToList(),
        // modelProposal.ResultSets?.Select(r => r.ToContract()).ToList()
        );

        return response;
    }
}

public class RejectProposalCommandHandler(
    IModelProposalRepository modelProposalRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<IModelEntity, bool>
{
    public async Task<Result<bool>> ExecuteAsync(
        IModelEntity command,
        CancellationToken ct = default
    )
    {
        await modelProposalRepository.RemoveById(command.ModelId, command.Id, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
