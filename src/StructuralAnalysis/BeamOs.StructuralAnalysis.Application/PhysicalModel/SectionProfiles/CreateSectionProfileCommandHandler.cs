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

        return sectionProfile.ToResponse(
            command.Body.AreaUnit.MapToAreaUnit(),
            command.Body.AreaMomentOfInertiaUnit.MapToAreaMomentOfInertiaUnit()
        );
    }
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
public static partial class CreateSectionProfileCommandMapper
{
    public static partial SectionProfile ToDomainObject(this CreateSectionProfileCommand command);

    public static SectionProfileResponse ToResponse(
        this SectionProfile entity,
        AreaUnit areaUnit,
        AreaMomentOfInertiaUnit areaMomentOfInertiaUnit
    ) =>
        ToResponse(
            entity,
            entity.Area.As(areaUnit),
            entity.StrongAxisMomentOfInertia.As(areaMomentOfInertiaUnit),
            entity.WeakAxisMomentOfInertia.As(areaMomentOfInertiaUnit),
            entity.PolarMomentOfInertia.As(areaMomentOfInertiaUnit),
            entity.StrongAxisShearArea.As(areaUnit),
            entity.WeakAxisShearArea.As(areaUnit),
            areaUnit,
            areaMomentOfInertiaUnit
        );

    private static partial SectionProfileResponse ToResponse(
        this SectionProfile entity,
        double area,
        double strongAxisMomentOfInertia,
        double weakAxisMomentOfInertia,
        double polarMomentOfInertia,
        double strongAxisShearArea,
        double weakAxisShearArea,
        AreaUnit areaUnit,
        AreaMomentOfInertiaUnit areaMomentOfInertiaUnit
    );
}

public readonly struct CreateSectionProfileCommand
    : IModelResourceRequest<CreateSectionProfileRequest>
{
    public Guid ModelId { get; init; }
    public CreateSectionProfileRequest Body { get; init; }
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
    public Area StrongAxisShearArea =>
        new(this.Body.StrongAxisShearArea, this.Body.AreaUnit.MapToAreaUnit());
    public Area WeakAxisShearArea =>
        new(this.Body.WeakAxisShearArea, this.Body.AreaUnit.MapToAreaUnit());
    public int? Id => this.Body.Id;
}
