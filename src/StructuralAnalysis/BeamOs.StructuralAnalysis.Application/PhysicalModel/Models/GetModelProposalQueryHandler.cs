using BeamOs.Common.Application;
using BeamOs.Common.Contracts;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

public sealed class GetModelProposalQueryHandler(IModelProposalRepository modelProposalRepository)
    : IQueryHandler<IModelEntity, ModelProposalContract>
{
    public async Task<Result<ModelProposalContract>> ExecuteAsync(
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
