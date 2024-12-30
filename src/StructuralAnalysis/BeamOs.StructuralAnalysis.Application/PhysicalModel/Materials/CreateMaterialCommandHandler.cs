using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Application.PhysicalModel.Materials;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;

public class CreateMaterialCommandHandler(
    IMaterialRepository materialRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<CreateMaterialCommand, MaterialResponse>
{
    public async Task<Result<MaterialResponse>> ExecuteAsync(
        CreateMaterialCommand command,
        CancellationToken ct = default
    )
    {
        Material material = command.ToDomainObject();
        materialRepository.Add(material);
        await unitOfWork.SaveChangesAsync(ct);

        return material.ToResponse();
    }
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
public static partial class CreateMaterialCommandMapper
{
    public static partial Material ToDomainObject(this CreateMaterialCommand command);

    public static partial MaterialResponse ToResponse(this Material entity);
}

public readonly struct CreateMaterialCommand : IModelResourceRequest<CreateMaterialRequest>
{
    public Guid ModelId { get; init; }
    public CreateMaterialRequest Body { get; init; }
    public PressureContract ModulusOfElasticity => this.Body.ModulusOfElasticity;
    public PressureContract ModulusOfRigidity => this.Body.ModulusOfRigidity;
    public int? Id => this.Body.Id;
}
