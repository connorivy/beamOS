using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;

internal class BatchPutNodeCommandHandler(
    INodeRepository repository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<NodeId, Node, ModelResourceRequest<PutNodeRequest[]>, PutNodeRequest>(
        repository,
        unitOfWork
    )
{
    protected override Node ToDomainObject(ModelId modelId, PutNodeRequest putRequest) =>
        new ModelResourceWithIntIdRequest<NodeData>(
            modelId,
            putRequest.Id,
            putRequest
        ).ToDomainObject();
}

internal class BatchPutInternalNodeCommandHandler(
    IInternalNodeRepository repository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<
        NodeId,
        InternalNode,
        ModelResourceRequest<InternalNodeContract[]>,
        InternalNodeContract
    >(repository, unitOfWork)
{
    protected override InternalNode ToDomainObject(
        ModelId modelId,
        InternalNodeContract putRequest
    ) => new ModelResourceRequest<InternalNodeContract>(modelId, putRequest).ToDomainObject();
}
