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
        var value = await this.HandleBimFirstModelCreation(command, model, ct);
        if (value.IsError)
        {
            return value.Error;
        }

        if (command.Options.IsTempModel)
        {
            modelRepository.AddTempModel(model);
        }
        else
        {
            modelRepository.Add(model);
        }

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

    private async Task<Result> HandleBimFirstModelCreation(
        CreateModelRequest command,
        Model model,
        CancellationToken ct
    )
    {
        if (model.Settings.WorkflowSettings.ModelingMode is not ModelingMode.BimFirst)
        {
            return Result.Success;
        }

        Model bimSourceModel;
        if (model.BimSourceModelId.HasValue)
        {
            var possibleBimSourceModel = await modelRepository.GetSingle(
                model.BimSourceModelId.Value,
                ct
            );
            if (possibleBimSourceModel is null)
            {
                return BeamOsError.NotFound(
                    description: $"BIM source model with ID {model.BimSourceModelId.Value} not found."
                );
            }
            if (
                possibleBimSourceModel.Settings.WorkflowSettings.ModelingMode
                != ModelingMode.BimFirstSource
            )
            {
                return BeamOsError.Validation(
                    description: $"BIM source model with ID {model.BimSourceModelId.Value} is not in BIM First Source modeling mode."
                );
            }
            bimSourceModel = possibleBimSourceModel;
        }
        else
        {
            bimSourceModel = (command with { Id = null }).ToDomainObject();
            bimSourceModel.Settings.WorkflowSettings.ModelingMode = ModelingMode.BimFirstSource;
            bimSourceModel.Name = $"BIM Source Model for {model.Name}";
            bimSourceModel.Description =
                $"This is an auto-generated BIM Source Model for {model.Name}. When you push changes from your BIM model, they will be applied to this model and then a change model request will be created for {model.Name}.";
            var defaultMaterial = new Material(bimSourceModel.Id, new(), new())
            {
                Model = bimSourceModel,
            };
            var defaultSectionProfile = new SectionProfile(
                bimSourceModel.Id,
                "Unset Section Profile",
                new(),
                new(),
                new(),
                new(),
                new(),
                new(),
                null,
                null
            )
            {
                Model = bimSourceModel,
            };
            bimSourceModel.Materials = [defaultMaterial];
            bimSourceModel.SectionProfiles = [defaultSectionProfile];

            modelRepository.Add(bimSourceModel);
        }
        bimSourceModel.Settings.WorkflowSettings.BimFirstModelIds ??= [];
        bimSourceModel.Settings.WorkflowSettings.BimFirstModelIds.Add(model.Id);
        model.BimSourceModel = bimSourceModel;

        return Result.Success;
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

    public ModelResponse Map(Model source)
    {
        var model = this.ToResponse(source);
        return model with
        {
            Settings = model.Settings with
            {
                WorkflowSettings = model.Settings.WorkflowSettings with
                {
                    BimSourceModelId = source.BimSourceModelId,
                },
            },
        };
    }

    private partial ModelResponse ToResponse(Model source);
}
