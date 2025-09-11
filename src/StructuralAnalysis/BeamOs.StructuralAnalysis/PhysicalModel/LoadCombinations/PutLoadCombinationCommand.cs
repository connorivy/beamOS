using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;

internal sealed class PutLoadCombinationCommandHandler(
    ILoadCombinationRepository repository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : PutCommandHandlerBase<
        LoadCombinationId,
        LoadCombination,
        ModelResourceWithIntIdRequest<LoadCombinationData>,
        LoadCombinationContract
    >(repository, unitOfWork)
{
    protected override LoadCombination ToDomainObject(
        ModelResourceWithIntIdRequest<LoadCombinationData> putCommand
    ) => putCommand.ToDomainObject();

    protected override LoadCombinationContract ToResponse(LoadCombination entity) =>
        entity.ToResponse();
}

internal sealed class BatchPutLoadCombinationCommandHandler(
    ILoadCombinationRepository repository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<
        LoadCombinationId,
        Domain.PhysicalModel.LoadCombinations.LoadCombination,
        ModelResourceRequest<LoadCombinationContract[]>,
        LoadCombinationContract
    >(repository, unitOfWork)
{
    protected override Domain.PhysicalModel.LoadCombinations.LoadCombination ToDomainObject(
        ModelId modelId,
        LoadCombinationContract putRequest
    ) =>
        new ModelResourceWithIntIdRequest<LoadCombinationData>(
            modelId,
            putRequest.Id,
            putRequest
        ).ToDomainObject();
}
