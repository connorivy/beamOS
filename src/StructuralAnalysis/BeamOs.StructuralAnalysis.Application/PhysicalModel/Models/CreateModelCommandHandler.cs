using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.CsSdk.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using Riok.Mapperly.Abstractions;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

public class CreateModelCommandHandler(
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
        await unitOfWork.SaveChangesAsync(ct);

        return ModelToResponseMapper.Create(model.Settings.UnitSettings).Map(model);
        //return model.ToResponse();
    }
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
public static partial class CreateModelCommandMapper
{
    public static partial Model ToDomainObject(this CreateModelRequest command);
}

[Mapper]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
[UseStaticMapper(typeof(UnitsNetMappersJustEnums))]
public partial class ModelToResponseMapper : AbstractMapperProvidedUnits<Model, ModelResponse>
{
    [Obsolete()]
    public ModelToResponseMapper()
        : base(null) { }

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
