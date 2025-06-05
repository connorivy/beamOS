using System.Diagnostics.CodeAnalysis;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;

public class PutNodeCommandHandler(
    INodeRepository nodeRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<PutNodeCommand, NodeResponse>
{
    public async Task<Result<NodeResponse>> ExecuteAsync(
        PutNodeCommand command,
        CancellationToken ct = default
    )
    {
        Node node = command.ToDomainObject();
        nodeRepository.Put(node);
        await unitOfWork.SaveChangesAsync(ct);

        return node.ToResponse();
    }
}

public sealed class PutInternalNodeCommandHandler(
    IInternalNodeRepository nodeRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : PutCommandHandlerBase<
        NodeId,
        InternalNode,
        ModelResourceRequest<InternalNodeData>,
        InternalNodeContract
    >(nodeRepository, unitOfWork)
{
    protected override InternalNode ToDomainObject(
        ModelResourceRequest<InternalNodeData> putCommand
    ) => putCommand.ToDomainObject();

    protected override InternalNodeContract ToResponse(InternalNode entity) => entity.ToResponse();
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
public static partial class PutNodeCommandMapper
{
    public static partial Node ToDomainObject(this PutNodeCommand command);

    public static partial NodeResponse ToResponse(this PutNodeCommand command);

    public static partial PutNodeRequest ToRequest(this PutNodeCommand command);

    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    public static partial InternalNode ToDomainObject(
        this ModelResourceRequest<InternalNodeData> command
    );

    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    public static partial InternalNode ToDomainObject(
        this ModelResourceRequest<InternalNodeContract> command
    );
}

public readonly struct PutNodeCommand : IModelResourceWithIntIdRequest<NodeData>
{
    public int Id { get; init; }
    public Guid ModelId { get; init; }
    public NodeData Body { get; init; }
    public Point LocationPoint => this.Body.LocationPoint;
    public Restraint? Restraint => this.Body.Restraint;

    public PutNodeCommand() { }

    [SetsRequiredMembers]
    public PutNodeCommand(Guid modelId, PutNodeRequest putNodeRequest)
    {
        this.Id = putNodeRequest.Id;
        this.ModelId = modelId;
        this.Body = putNodeRequest;
    }
}
