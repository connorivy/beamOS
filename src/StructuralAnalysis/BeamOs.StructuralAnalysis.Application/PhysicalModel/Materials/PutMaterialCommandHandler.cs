using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;

public class PutMaterialCommandHandler(
    IMaterialRepository materialRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<PutMaterialCommand, MaterialResponse>
{
    public async Task<Result<MaterialResponse>> ExecuteAsync(
        PutMaterialCommand command,
        CancellationToken ct = default
    )
    {
        Material material = command.ToDomainObject();
        await materialRepository.Put(material);
        await unitOfWork.SaveChangesAsync(ct);

        return material.ToResponse(command.Body.PressureUnit.MapToPressureUnit());
    }
}

public class BatchPutMaterialCommandHandler(
    IMaterialRepository materialRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<MaterialId, Material, BatchPutMaterialCommand, PutMaterialRequest>(
        materialRepository,
        unitOfWork
    )
{
    protected override Material ToDomainObject(ModelId modelId, PutMaterialRequest putRequest) =>
        new PutMaterialCommand(modelId, putRequest).ToDomainObject();
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
public static partial class PutMaterialCommandMapper
{
    public static partial Material ToDomainObject(this PutMaterialCommand command);
}

public readonly struct PutMaterialCommand : IModelResourceWithIntIdRequest<MaterialData>
{
    public int Id { get; init; }
    public Guid ModelId { get; init; }
    public MaterialData Body { get; init; }
    public Pressure ModulusOfElasticity =>
        new(this.Body.ModulusOfElasticity, this.Body.PressureUnit.MapToPressureUnit());
    public Pressure ModulusOfRigidity =>
        new(this.Body.ModulusOfRigidity, this.Body.PressureUnit.MapToPressureUnit());

    public PutMaterialCommand() { }

    public PutMaterialCommand(ModelId modelId, PutMaterialRequest putMaterialRequest)
    {
        this.Id = putMaterialRequest.Id;
        this.ModelId = modelId;
        this.Body = putMaterialRequest;
    }

    public MaterialResponse ToResponse()
    {
        return new MaterialResponse(
            this.Id,
            this.ModelId,
            this.ModulusOfElasticity.Value,
            this.ModulusOfRigidity.Value,
            this.Body.PressureUnit
        );
    }
}

public readonly struct BatchPutMaterialCommand : IModelResourceRequest<PutMaterialRequest[]>
{
    public Guid ModelId { get; init; }
    public PutMaterialRequest[] Body { get; init; }
}
