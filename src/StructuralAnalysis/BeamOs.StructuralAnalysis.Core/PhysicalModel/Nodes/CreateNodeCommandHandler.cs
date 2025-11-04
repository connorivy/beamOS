using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;

internal class CreateNodeCommandHandler(
    // INodeDefinitionRepository nodeRepository,
    INodeRepository nodeRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceRequest<CreateNodeRequest>, NodeResponse>
{
    public async Task<Result<NodeResponse>> ExecuteAsync(
        ModelResourceRequest<CreateNodeRequest> command,
        CancellationToken ct = default
    )
    {
        Node node = command.ToDomainObject();

        nodeRepository.Add(node);
        // The OctreeNodeId is automatically assigned by the ModelEntityIdIncrementingInterceptor
        // to match the Node's ID when SaveChangesAsync is called
        await unitOfWork.SaveChangesAsync(ct);

        return node.ToResponse();
    }
}

internal sealed class CreateInternalNodeCommandHandler(
    INodeDefinitionRepository nodeRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceRequest<CreateInternalNodeRequest>, InternalNodeContract>
{
    public async Task<Result<InternalNodeContract>> ExecuteAsync(
        ModelResourceRequest<CreateInternalNodeRequest> command,
        CancellationToken ct = default
    )
    {
        var node = command.ToDomainObject();
        nodeRepository.Add(node);
        await unitOfWork.SaveChangesAsync(ct);

        return node.ToResponse();
    }
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
internal static partial class CreateNodeCommandMapper
{
    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    public static partial Node ToDomainObject(this ModelResourceRequest<CreateNodeRequest> command);

    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    public static partial CreateNodeRequest ToRequest(
        this ModelResourceRequest<CreateNodeRequest> entity
    );

    public static partial NodeResponse ToResponse(this Node entity);

    [MapperIgnoreTarget(nameof(CreateNodeRequest.Id))]
    public static partial CreateNodeRequest ToRequest(this NodeResponse entity);

    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    public static partial InternalNode ToDomainObject(
        this ModelResourceRequest<CreateInternalNodeRequest> command
    );

    public static partial InternalNodeContract ToResponse(this InternalNode entity);
}
