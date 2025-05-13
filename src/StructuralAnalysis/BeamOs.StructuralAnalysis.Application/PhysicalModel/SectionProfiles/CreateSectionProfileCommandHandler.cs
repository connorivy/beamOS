using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;

public class CreateSectionProfileCommandHandler(
    ISectionProfileRepository sectionProfileRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<CreateSectionProfileCommand, SectionProfileResponse>
{
    public async Task<Result<SectionProfileResponse>> ExecuteAsync(
        CreateSectionProfileCommand command,
        CancellationToken ct = default
    )
    {
        SectionProfile sectionProfile = command.ToDomainObject();
        sectionProfileRepository.Add(sectionProfile);
        await unitOfWork.SaveChangesAsync(ct);

        return sectionProfile.ToResponse(command.Body.LengthUnit.MapToLengthUnit());
    }
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
public static partial class CreateSectionProfileCommandMapper
{
    public static partial SectionProfile ToDomainObject(this CreateSectionProfileCommand command);

    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    public static partial SectionProfileFromLibrary ToDomainObject(
        this ModelResourceRequest<SectionProfileFromLibraryData> command
    );

    public static SectionProfileResponse ToResponse(
        this SectionProfile entity,
        LengthUnit lengthUnit
    )
    {
        var areaUnit = lengthUnit.ToArea();
        var areaMomentOfInertiaUnit = lengthUnit.ToAreaMomentOfInertiaUnit();
        var volumeUnit = lengthUnit.ToVolume();
        return ToResponse(
            entity,
            entity.Area.As(areaUnit),
            entity.StrongAxisMomentOfInertia.As(areaMomentOfInertiaUnit),
            entity.WeakAxisMomentOfInertia.As(areaMomentOfInertiaUnit),
            entity.PolarMomentOfInertia.As(areaMomentOfInertiaUnit),
            entity.StrongAxisPlasticSectionModulus.As(volumeUnit),
            entity.WeakAxisPlasticSectionModulus.As(volumeUnit),
            entity.StrongAxisShearArea?.As(areaUnit),
            entity.WeakAxisShearArea?.As(areaUnit),
            lengthUnit
        );
    }

    private static partial SectionProfileResponse ToResponse(
        this SectionProfile entity,
        double area,
        double strongAxisMomentOfInertia,
        double weakAxisMomentOfInertia,
        double polarMomentOfInertia,
        double strongAxisPlasticSectionModulus,
        double weakAxisPlasticSectionModulus,
        double? strongAxisShearArea,
        double? weakAxisShearArea,
        LengthUnit lengthUnit
    );
}

public readonly struct CreateSectionProfileCommand
    : IModelResourceRequest<CreateSectionProfileRequest>
{
    public Guid ModelId { get; init; }
    public CreateSectionProfileRequest Body { get; init; }
    public string Name => this.Body.Name;
    public Area Area => new(this.Body.Area, this.Body.AreaUnit.MapToAreaUnit());
    public AreaMomentOfInertia StrongAxisMomentOfInertia =>
        new(
            this.Body.StrongAxisMomentOfInertia,
            this.Body.AreaMomentOfInertiaUnit.MapToAreaMomentOfInertiaUnit()
        );
    public AreaMomentOfInertia WeakAxisMomentOfInertia =>
        new(
            this.Body.WeakAxisMomentOfInertia,
            this.Body.AreaMomentOfInertiaUnit.MapToAreaMomentOfInertiaUnit()
        );
    public AreaMomentOfInertia PolarMomentOfInertia =>
        new(
            this.Body.PolarMomentOfInertia,
            this.Body.AreaMomentOfInertiaUnit.MapToAreaMomentOfInertiaUnit()
        );
    public Volume StrongAxisPlasticSectionModulus =>
        new(this.Body.StrongAxisPlasticSectionModulus, this.Body.VolumeUnit.MapToVolumeUnit());
    public Volume WeakAxisPlasticSectionModulus =>
        new(this.Body.WeakAxisPlasticSectionModulus, this.Body.VolumeUnit.MapToVolumeUnit());
    public Area? StrongAxisShearArea =>
        this.Body.StrongAxisShearArea is null
            ? null
            : new(this.Body.StrongAxisShearArea.Value, this.Body.AreaUnit.MapToAreaUnit());
    public Area? WeakAxisShearArea =>
        this.Body.WeakAxisShearArea is null
            ? null
            : new(this.Body.WeakAxisShearArea.Value, this.Body.AreaUnit.MapToAreaUnit());
    public int? Id => this.Body.Id;
}
