using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using Riok.Mapperly.Abstractions;

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

        return sectionProfile.ToResponse();
    }
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
public static partial class CreateSectionProfileCommandMapper
{
    public static partial SectionProfile ToDomainObject(this CreateSectionProfileCommand command);

    public static partial SectionProfileResponse ToResponse(this SectionProfile entity);
}

public readonly struct CreateSectionProfileCommand
    : IModelResourceRequest<CreateSectionProfileRequest>
{
    public Guid ModelId { get; init; }
    public CreateSectionProfileRequest Body { get; init; }
    public AreaContract Area => this.Body.Area;
    public AreaMomentOfInertiaContract StrongAxisMomentOfInertia =>
        this.Body.StrongAxisMomentOfInertia;
    public AreaMomentOfInertiaContract WeakAxisMomentOfInertia => this.Body.WeakAxisMomentOfInertia;
    public AreaMomentOfInertiaContract PolarMomentOfInertia => this.Body.PolarMomentOfInertia;
    public AreaContract StrongAxisShearArea => this.Body.StrongAxisShearArea;
    public AreaContract WeakAxisShearArea => this.Body.WeakAxisShearArea;
}
