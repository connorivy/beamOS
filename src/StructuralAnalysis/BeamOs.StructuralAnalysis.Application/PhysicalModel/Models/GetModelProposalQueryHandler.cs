using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

public sealed class GetModelProposalQueryHandler(
    IModelProposalRepository modelProposalRepository,
    IElement1dRepository element1DRepository
) : IQueryHandler<IModelEntity, ModelProposalResponse>
{
    public async Task<Result<ModelProposalResponse>> ExecuteAsync(
        IModelEntity command,
        CancellationToken ct = default
    )
    {
        var proposal = await modelProposalRepository.GetSingle(command.ModelId, command.Id, ct);
        if (proposal is null)
        {
            return BeamOsError.NotFound(
                description: $"Model proposal with id {command.Id} not found"
            );
        }
        var changedNodes = proposal
            .NodeProposals.Select(p => p.ExistingId)
            .OfType<NodeId>()
            .ToList();

        var element1dsModifiedBecauseOfNodeChange = await element1DRepository.GetMany(
            command.ModelId,
            changedNodes,
            ct
        );

        var proposalResponse = proposal.ToContract();

        return proposalResponse with
        {
            Element1dsModifiedBecauseOfNodeChange =
            [
                .. element1dsModifiedBecauseOfNodeChange.Select(e => e.Id.Id),
            ],
        };
    }
}

public sealed class GetModelProposalsQueryHandler(IModelProposalRepository modelProposalRepository)
    : IQueryHandler<Guid, List<ModelProposalInfo>>
{
    public async Task<Result<List<ModelProposalInfo>>> ExecuteAsync(
        Guid command,
        CancellationToken ct = default
    )
    {
        var proposals = await modelProposalRepository.GetMany(command, null, ct);
        if (proposals is null)
        {
            return BeamOsError.NotFound(
                description: $"No proposals found for model with id {command}"
            );
        }

        return proposals.Select(p => p.ToInfoContract()).ToList();
    }
}
