using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;

internal class PutElement1dCommandHandler(
    IElement1dRepository element1dRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceWithIntIdRequest<Element1dData>, Element1dResponse>
{
    public async Task<Result<Element1dResponse>> ExecuteAsync(
        ModelResourceWithIntIdRequest<Element1dData> command,
        CancellationToken ct = default
    )
    {
        Element1d element1d = command.ToDomainObject();
        await element1dRepository.Put(element1d);
        await unitOfWork.SaveChangesAsync(ct);

        return element1d.ToResponse();
    }
}

internal class BatchPutElement1dCommandHandler(
    IElement1dRepository element1dRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<
        Element1dId,
        Element1d,
        ModelResourceRequest<PutElement1dRequest[]>,
        PutElement1dRequest
    >(element1dRepository, unitOfWork)
{
    protected override Element1d ToDomainObject(ModelId modelId, PutElement1dRequest putRequest) =>
        new ModelResourceWithIntIdRequest<Element1dData>(
            modelId,
            putRequest.Id,
            putRequest
        ).ToDomainObject();
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
internal static partial class PutElement1dCommandMapper
{
    [MapNestedProperties(nameof(ModelResourceWithIntIdRequest<>.Body))]
    public static partial Element1dResponse ToResponse(
        this ModelResourceWithIntIdRequest<Element1dData> command
    );

    [MapNestedProperties(nameof(ModelResourceWithIntIdRequest<>.Body))]
    public static partial Element1d ToDomainObject(
        this ModelResourceWithIntIdRequest<Element1dData> command
    );
}
