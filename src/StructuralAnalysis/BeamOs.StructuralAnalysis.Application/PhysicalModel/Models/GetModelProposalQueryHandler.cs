using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

public sealed class GetModelProposalQueryHandler(IModelProposalRepository modelProposalRepository)
    : IQueryHandler<IModelEntity, ModelProposalResponse>
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

        return proposal.ToContract();
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
        if (proposals is null || proposals.Count == 0)
        {
            return BeamOsError.NotFound(
                description: $"No proposals found for model with id {command}"
            );
        }

        return proposals.Select(p => p.ToInfoContract()).ToList();
    }
}
