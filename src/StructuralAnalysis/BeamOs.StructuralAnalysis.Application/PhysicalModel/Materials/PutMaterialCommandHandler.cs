using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
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
        materialRepository.Put(material);
        await unitOfWork.SaveChangesAsync(ct);

        return material.ToResponse();
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

public readonly struct PutMaterialCommand : IModelResourceWithIntIdRequest<MaterialRequestData>
{
    public int Id { get; init; }
    public Guid ModelId { get; init; }
    public MaterialRequestData Body { get; init; }
    public PressureContract ModulusOfElasticity => this.Body.ModulusOfElasticity;
    public PressureContract ModulusOfRigidity => this.Body.ModulusOfRigidity;

    public PutMaterialCommand() { }

    public PutMaterialCommand(ModelId modelId, PutMaterialRequest putMaterialRequest)
    {
        this.Id = putMaterialRequest.Id;
        this.ModelId = modelId;
        this.Body = putMaterialRequest;
    }
}

public readonly struct BatchPutMaterialCommand : IModelResourceRequest<PutMaterialRequest[]>
{
    public Guid ModelId { get; init; }
    public PutMaterialRequest[] Body { get; init; }
}
