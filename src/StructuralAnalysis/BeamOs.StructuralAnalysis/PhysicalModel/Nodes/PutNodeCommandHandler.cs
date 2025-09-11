using System.Diagnostics.CodeAnalysis;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.NodeResults;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;

internal class PutNodeCommandHandler(
    // INodeRepository nodeRepository,
    INodeDefinitionRepository nodeDefinitionRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceWithIntIdRequest<NodeData>, NodeResponse>
{
    public async Task<Result<NodeResponse>> ExecuteAsync(
        ModelResourceWithIntIdRequest<NodeData> command,
        CancellationToken ct = default
    )
    {
        Node node = command.ToDomainObject();
        await nodeDefinitionRepository.Put(node);
        await unitOfWork.SaveChangesAsync(ct);

        return node.ToResponse();
    }
}

internal sealed class PutInternalNodeCommandHandler(
    INodeDefinitionRepository nodeRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : PutCommandHandlerBase<
        NodeId,
        InternalNode,
        ModelResourceWithIntIdRequest<InternalNodeData>,
        InternalNodeContract
    >(nodeRepository, unitOfWork)
{
    protected override InternalNode ToDomainObject(
        ModelResourceWithIntIdRequest<InternalNodeData> putCommand
    ) => putCommand.ToDomainObject();

    protected override InternalNodeContract ToResponse(InternalNode entity) => entity.ToResponse();
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
internal static partial class PutNodeCommandMapper
{
    [MapNestedProperties(nameof(ModelResourceWithIntIdRequest<>.Body))]
    public static partial Node ToDomainObject(this ModelResourceWithIntIdRequest<NodeData> command);

    [MapNestedProperties(nameof(ModelResourceWithIntIdRequest<>.Body))]
    public static partial NodeResponse ToResponse(
        this ModelResourceWithIntIdRequest<NodeData> command
    );

    [MapNestedProperties(nameof(ModelResourceWithIntIdRequest<>.Body))]
    public static partial PutNodeRequest ToRequest(
        this ModelResourceWithIntIdRequest<NodeData> command
    );

    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    public static partial InternalNode ToDomainObject(
        this ModelResourceWithIntIdRequest<InternalNodeData> command
    );

    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    public static partial InternalNode ToDomainObject(
        this ModelResourceRequest<InternalNodeContract> command
    );
}
