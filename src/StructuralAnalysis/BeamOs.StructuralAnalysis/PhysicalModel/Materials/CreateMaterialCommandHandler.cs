using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;

internal class CreateMaterialCommandHandler(
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

        return material.ToResponse(command.Body.PressureUnit.MapToPressureUnit());
    }
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
internal static partial class CreateMaterialStaticMapper
{
    public static partial Material ToDomainObject(this CreateMaterialCommand command);

    public static MaterialResponse ToResponse(this Material command, PressureUnit pressureUnit) =>
        command.ToResponse(
            command.ModulusOfElasticity.As(pressureUnit),
            command.ModulusOfRigidity.As(pressureUnit),
            pressureUnit
        );

    private static partial MaterialResponse ToResponse(
        this Material command,
        double modulusOfElasticity,
        double modulusOfRigidity,
        PressureUnit pressureUnit
    );
}

internal readonly struct CreateMaterialCommand : IModelResourceRequest<CreateMaterialRequest>
{
    public Guid ModelId { get; init; }
    public CreateMaterialRequest Body { get; init; }
    public Pressure ModulusOfElasticity =>
        new(this.Body.ModulusOfElasticity, this.Body.PressureUnit.MapToPressureUnit());
    public Pressure ModulusOfRigidity =>
        new(this.Body.ModulusOfRigidity, this.Body.PressureUnit.MapToPressureUnit());
    public int? Id => this.Body.Id;
}
