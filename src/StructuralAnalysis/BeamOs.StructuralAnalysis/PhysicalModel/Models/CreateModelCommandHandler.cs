using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.CsSdk.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

internal class CreateModelCommandHandler(
    IModelRepository modelRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<CreateModelRequest, ModelResponse>
{
    public async Task<Result<ModelResponse>> ExecuteAsync(
        CreateModelRequest command,
        CancellationToken ct = default
    )
    {
        Model model = command.ToDomainObject();
        modelRepository.Add(model);
        try
        {
            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (UniqueConstraintException)
        {
            return BeamOsError.Conflict(description: $"Model with ID {model.Id} already exists.");
        }

        return ModelToResponseMapper.Create(model.Settings.UnitSettings).Map(model);
    }
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
internal static partial class CreateModelCommandMapper
{
    public static partial Model ToDomainObject(this CreateModelRequest command);

    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    public static partial Model ToDomainObject(this ModelResourceRequest<ModelInfoData> command);
}

[Mapper]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
[UseStaticMapper(typeof(UnitsNetMappersJustEnums))]
internal partial class ModelToResponseMapper : AbstractMapperProvidedUnits<Model, ModelResponse>
{
    // [Obsolete()]
    // public ModelToResponseMapper()
    //     : base(null) { }

    private ModelToResponseMapper(UnitSettings unitSettings)
        : base(unitSettings) { }

    public static ModelToResponseMapper Create(UnitSettings unitSettings)
    {
        return new(unitSettings);
    }

    public MaterialResponse ToResponse(Material entity) =>
        this.ToResponse(entity, this.PressureUnit);

    private partial MaterialResponse ToResponse(Material entity, PressureUnit pressureUnit);

    public SectionProfileResponse ToResponse(SectionProfile entity) =>
        this.ToResponse(entity, this.LengthUnit);

    private partial SectionProfileResponse ToResponse(SectionProfile entity, LengthUnit lengthUnit);

    public ModelResponse Map(Model source) => this.ToResponse(source);

    private partial ModelResponse ToResponse(Model source);
}
