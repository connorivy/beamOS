using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;

internal class PutPointLoadCommandHandler(
    IPointLoadRepository pointLoadRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceWithIntIdRequest<PointLoadData>, PointLoadResponse>
{
    public async Task<Result<PointLoadResponse>> ExecuteAsync(
        ModelResourceWithIntIdRequest<PointLoadData> command,
        CancellationToken ct = default
    )
    {
        PointLoad pointLoad = command.ToDomainObject();
        await pointLoadRepository.Put(pointLoad);
        await unitOfWork.SaveChangesAsync(ct);

        return pointLoad.ToResponse();
    }
}

internal class BatchPutPointLoadCommandHandler(
    IPointLoadRepository pointLoadRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<
        PointLoadId,
        PointLoad,
        ModelResourceRequest<PutPointLoadRequest[]>,
        PutPointLoadRequest
    >(pointLoadRepository, unitOfWork)
{
    protected override PointLoad ToDomainObject(ModelId modelId, PutPointLoadRequest putRequest) =>
        new ModelResourceWithIntIdRequest<PointLoadData>(
            modelId,
            putRequest.Id,
            putRequest
        ).ToDomainObject();
}

[Mapper(PreferParameterlessConstructors = false)]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
internal static partial class PutPointLoadCommandMapper
{
    [MapNestedProperties(nameof(ModelResourceWithIntIdRequest<>.Body))]
    public static partial PointLoad ToDomainObject(
        this ModelResourceWithIntIdRequest<PointLoadData> command
    );
}
