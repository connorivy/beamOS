using BeamOs.Common.Application;
using BeamOs.Common.Contracts;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;

internal class GetInternalNodeCommandHandler(INodeDefinitionRepository repository)
    : ICommandHandler<IModelEntity, InternalNodeContract>
{
    public async Task<Result<InternalNodeContract>> ExecuteAsync(
        IModelEntity query,
        CancellationToken ct = default
    )
    {
        var nodeDefinition = await repository.GetSingle(query.ModelId, query.Id, ct);
        if (nodeDefinition is null)
        {
            return BeamOsError.NotFound(
                description: $"Node with id {query.Id} not found on model with id {query.ModelId}."
            );
        }

        if (nodeDefinition.CastToInternalNodeIfApplicable() is not InternalNode internalNode)
        {
            return BeamOsError.InvalidOperation(
                description: $"Node with id {query.Id} is not an internal node."
            );
        }

        return internalNode.ToResponse();
    }
}
