using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;

internal class PutMaterialCommandHandler(
    IMaterialRepository materialRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceWithIntIdRequest<MaterialData>, MaterialResponse>
{
    public async Task<Result<MaterialResponse>> ExecuteAsync(
        ModelResourceWithIntIdRequest<MaterialData> command,
        CancellationToken ct = default
    )
    {
        Material material = command.ToDomainObject();
        await materialRepository.Put(material);
        await unitOfWork.SaveChangesAsync(ct);

        return material.ToResponse(command.Body.PressureUnit.MapToPressureUnit());
    }
}

internal class BatchPutMaterialCommandHandler(
    IMaterialRepository materialRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<
        MaterialId,
        Material,
        ModelResourceRequest<PutMaterialRequest[]>,
        PutMaterialRequest
    >(materialRepository, unitOfWork)
{
    protected override Material ToDomainObject(ModelId modelId, PutMaterialRequest putRequest) =>
        new ModelResourceWithIntIdRequest<MaterialData>(
            modelId,
            putRequest.Id,
            putRequest
        ).ToDomainObject();
}
