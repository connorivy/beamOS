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
) : ICommandHandler<ModelResourceRequest<CreateMaterialRequest>, MaterialResponse>
{
    public async Task<Result<MaterialResponse>> ExecuteAsync(
        ModelResourceRequest<CreateMaterialRequest> command,
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
    [MapProperty("Body." + nameof(MaterialData.ModulusOfElasticityInternal), "ModulusOfElasticity")]
    [MapProperty("Body." + nameof(MaterialData.ModulusOfRigidityInternal), "ModulusOfRigidity")]
    public static partial Material ToDomainObject(
        this ModelResourceWithIntIdRequest<MaterialData> command
    );

    [MapProperty("Body." + nameof(MaterialData.ModulusOfElasticityInternal), "ModulusOfElasticity")]
    [MapProperty("Body." + nameof(MaterialData.ModulusOfRigidityInternal), "ModulusOfRigidity")]
    public static partial Material ToDomainObject(
        this ModelResourceRequest<CreateMaterialRequest> command
    );

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
