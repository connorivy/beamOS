using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;

internal class CreateSectionProfileCommandHandler(
    ISectionProfileRepository sectionProfileRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceRequest<CreateSectionProfileRequest>, SectionProfileResponse>
{
    public async Task<Result<SectionProfileResponse>> ExecuteAsync(
        ModelResourceRequest<CreateSectionProfileRequest> command,
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
internal static partial class CreateSectionProfileCommandMapper
{
    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    [MapProperty("Body." + nameof(CreateSectionProfileRequest.AreaInternal), "Area")]
    [MapProperty(
        "Body." + nameof(CreateSectionProfileRequest.StrongAxisMomentOfInertiaInternal),
        nameof(SectionProfile.StrongAxisMomentOfInertia)
    )]
    [MapProperty(
        "Body." + nameof(CreateSectionProfileRequest.WeakAxisMomentOfInertiaInternal),
        nameof(SectionProfile.WeakAxisMomentOfInertia)
    )]
    [MapProperty(
        "Body." + nameof(CreateSectionProfileRequest.PolarMomentOfInertiaInternal),
        nameof(SectionProfile.PolarMomentOfInertia)
    )]
    [MapProperty(
        "Body." + nameof(CreateSectionProfileRequest.StrongAxisPlasticSectionModulusInternal),
        nameof(SectionProfile.StrongAxisPlasticSectionModulus)
    )]
    [MapProperty(
        "Body." + nameof(CreateSectionProfileRequest.WeakAxisPlasticSectionModulusInternal),
        nameof(SectionProfile.WeakAxisPlasticSectionModulus)
    )]
    [MapProperty(
        "Body." + nameof(CreateSectionProfileRequest.StrongAxisShearAreaInternal),
        nameof(SectionProfile.StrongAxisShearArea)
    )]
    [MapProperty(
        "Body." + nameof(CreateSectionProfileRequest.WeakAxisShearAreaInternal),
        nameof(SectionProfile.WeakAxisShearArea)
    )]
    public static partial SectionProfile ToDomainObject(
        this ModelResourceRequest<CreateSectionProfileRequest> command
    );

    public static SectionProfile ToDomainObject(
        this CreateSectionProfileRequest sectionProfileRequest,
        ModelId modelId
    ) =>
        new ModelResourceRequest<CreateSectionProfileRequest>(
            modelId,
            sectionProfileRequest
        ).ToDomainObject();

    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    [MapProperty("Body." + nameof(CreateSectionProfileRequest.AreaInternal), "Area")]
    [MapProperty(
        "Body." + nameof(CreateSectionProfileRequest.StrongAxisMomentOfInertiaInternal),
        nameof(SectionProfile.StrongAxisMomentOfInertia)
    )]
    [MapProperty(
        "Body." + nameof(CreateSectionProfileRequest.WeakAxisMomentOfInertiaInternal),
        nameof(SectionProfile.WeakAxisMomentOfInertia)
    )]
    [MapProperty(
        "Body." + nameof(CreateSectionProfileRequest.PolarMomentOfInertiaInternal),
        nameof(SectionProfile.PolarMomentOfInertia)
    )]
    [MapProperty(
        "Body." + nameof(CreateSectionProfileRequest.StrongAxisPlasticSectionModulusInternal),
        nameof(SectionProfile.StrongAxisPlasticSectionModulus)
    )]
    [MapProperty(
        "Body." + nameof(CreateSectionProfileRequest.WeakAxisPlasticSectionModulusInternal),
        nameof(SectionProfile.WeakAxisPlasticSectionModulus)
    )]
    [MapProperty(
        "Body." + nameof(CreateSectionProfileRequest.StrongAxisShearAreaInternal),
        nameof(SectionProfile.StrongAxisShearArea)
    )]
    [MapProperty(
        "Body." + nameof(CreateSectionProfileRequest.WeakAxisShearAreaInternal),
        nameof(SectionProfile.WeakAxisShearArea)
    )]
    public static partial SectionProfile ToDomainObject(
        this ModelResourceWithIntIdRequest<SectionProfileData> command
    );

    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    public static partial SectionProfileFromLibrary ToDomainObject(
        this ModelResourceRequest<SectionProfileFromLibraryData> command
    );

    public static partial SectionProfileFromLibraryContract ToResponse(
        this SectionProfileFromLibrary command
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
