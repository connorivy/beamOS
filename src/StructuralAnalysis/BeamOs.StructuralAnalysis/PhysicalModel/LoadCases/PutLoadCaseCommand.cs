using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;

internal sealed class BatchPutLoadCaseCommandHandler(
    ILoadCaseRepository repository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<
        LoadCaseId,
        Domain.PhysicalModel.LoadCases.LoadCase,
        ModelResourceRequest<LoadCaseContract[]>,
        LoadCaseContract
    >(repository, unitOfWork)
{
    protected override Domain.PhysicalModel.LoadCases.LoadCase ToDomainObject(
        ModelId modelId,
        LoadCaseContract putRequest
    ) =>
        new ModelResourceWithIntIdRequest<LoadCaseData>(
            modelId,
            putRequest.Id,
            putRequest
        ).ToDomainObject();
}

internal sealed class PutLoadCaseCommandHandler(
    ILoadCaseRepository repository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : PutCommandHandlerBase<
        LoadCaseId,
        Domain.PhysicalModel.LoadCases.LoadCase,
        ModelResourceWithIntIdRequest<LoadCaseData>,
        LoadCaseContract
    >(repository, unitOfWork)
{
    protected override Domain.PhysicalModel.LoadCases.LoadCase ToDomainObject(
        ModelResourceWithIntIdRequest<LoadCaseData> putCommand
    ) => putCommand.ToDomainObject();

    protected override LoadCaseContract ToResponse(
        Domain.PhysicalModel.LoadCases.LoadCase entity
    ) => entity.ToResponse();
}
