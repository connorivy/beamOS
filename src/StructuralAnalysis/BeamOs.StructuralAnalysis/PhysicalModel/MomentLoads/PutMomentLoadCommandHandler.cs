using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;

internal class PutMomentLoadCommandHandler(
    IMomentLoadRepository momentLoadRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceWithIntIdRequest<MomentLoadData>, MomentLoadResponse>
{
    public async Task<Result<MomentLoadResponse>> ExecuteAsync(
        ModelResourceWithIntIdRequest<MomentLoadData> command,
        CancellationToken ct = default
    )
    {
        MomentLoad momentLoad = command.ToDomainObject();
        await momentLoadRepository.Put(momentLoad);
        await unitOfWork.SaveChangesAsync(ct);

        return momentLoad.ToResponse();
    }
}

internal class BatchPutMomentLoadCommandHandler(
    IMomentLoadRepository momentLoadRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<
        MomentLoadId,
        MomentLoad,
        ModelResourceRequest<PutMomentLoadRequest[]>,
        PutMomentLoadRequest
    >(momentLoadRepository, unitOfWork)
{
    protected override MomentLoad ToDomainObject(
        ModelId modelId,
        PutMomentLoadRequest putRequest
    ) =>
        new ModelResourceWithIntIdRequest<MomentLoadData>(
            modelId,
            putRequest.Id,
            putRequest
        ).ToDomainObject();
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
internal static partial class PutMomentLoadCommandMapper
{
    [MapNestedProperties(nameof(ModelResourceWithIntIdRequest<>.Body))]
    public static partial MomentLoad ToDomainObject(
        this ModelResourceWithIntIdRequest<MomentLoadData> command
    );

    [MapNestedProperties(nameof(ModelResourceWithIntIdRequest<>.Body))]
    public static partial MomentLoadResponse ToResponse(
        this ModelResourceWithIntIdRequest<MomentLoadData> command
    );
}
