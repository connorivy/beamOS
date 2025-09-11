using System.Diagnostics.CodeAnalysis;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using Riok.Mapperly.Abstractions;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;

internal class PutSectionProfileCommandHandler(
    ISectionProfileRepository sectionProfileRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<PutSectionProfileCommand, SectionProfileResponse>
{
    public async Task<Result<SectionProfileResponse>> ExecuteAsync(
        PutSectionProfileCommand command,
        CancellationToken ct = default
    )
    {
        SectionProfile sectionProfile = command.ToDomainObject();
        await sectionProfileRepository.Put(sectionProfile);
        await unitOfWork.SaveChangesAsync(ct);

        return sectionProfile.ToResponse(command.Body.LengthUnit.MapToLengthUnit());
    }
}

internal class BatchPutSectionProfileCommandHandler(
    ISectionProfileRepository sectionProfileRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<
        SectionProfileId,
        SectionProfile,
        BatchPutSectionProfileCommand,
        PutSectionProfileRequest
    >(sectionProfileRepository, unitOfWork)
{
    protected override SectionProfile ToDomainObject(
        ModelId modelId,
        PutSectionProfileRequest putRequest
    ) => new PutSectionProfileCommand(modelId, putRequest).ToDomainObject();
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
internal static partial class PutSectionProfileCommandMapper
{
    public static partial SectionProfile ToDomainObject(this PutSectionProfileCommand command);

    public static partial SectionProfileFromLibrary ToDomainObject(
        this PutSectionProfileFromLibraryCommand command
    );
}

internal readonly struct PutSectionProfileCommand : IModelResourceWithIntIdRequest<SectionProfileData>
{
    public int Id { get; init; }
    public Guid ModelId { get; init; }
    public SectionProfileData Body { get; init; }
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
        this.Body.StrongAxisShearArea.HasValue
            ? new(this.Body.StrongAxisShearArea.Value, this.Body.AreaUnit.MapToAreaUnit())
            : null;
    public Area? WeakAxisShearArea =>
        this.Body.WeakAxisShearArea.HasValue
            ? new(this.Body.WeakAxisShearArea.Value, this.Body.AreaUnit.MapToAreaUnit())
            : null;

    public PutSectionProfileCommand() { }

    [SetsRequiredMembers]
    public PutSectionProfileCommand(
        ModelId modelId,
        PutSectionProfileRequest putSectionProfileRequest
    )
    {
        this.Id = putSectionProfileRequest.Id;
        this.ModelId = modelId;
        this.Body = putSectionProfileRequest;
    }

    public SectionProfileResponse ToResponse()
    {
        return new(
            this.Id,
            this.ModelId,
            this.Body.Name,
            this.Area.As(this.Body.AreaUnit.MapToAreaUnit()),
            this.StrongAxisMomentOfInertia.As(
                this.Body.AreaMomentOfInertiaUnit.MapToAreaMomentOfInertiaUnit()
            ),
            this.WeakAxisMomentOfInertia.As(
                this.Body.AreaMomentOfInertiaUnit.MapToAreaMomentOfInertiaUnit()
            ),
            this.PolarMomentOfInertia.As(
                this.Body.AreaMomentOfInertiaUnit.MapToAreaMomentOfInertiaUnit()
            ),
            this.StrongAxisPlasticSectionModulus.As(this.Body.VolumeUnit.MapToVolumeUnit()),
            this.WeakAxisPlasticSectionModulus.As(this.Body.VolumeUnit.MapToVolumeUnit()),
            this.StrongAxisShearArea?.As(this.Body.AreaUnit.MapToAreaUnit()),
            this.WeakAxisShearArea?.As(this.Body.AreaUnit.MapToAreaUnit()),
            this.Body.LengthUnit
        );
    }

    public PutSectionProfileRequest ToRequest()
    {
        return new(this.Id, this.Body);
    }
}

internal readonly struct BatchPutSectionProfileCommand
    : IModelResourceRequest<PutSectionProfileRequest[]>
{
    public Guid ModelId { get; init; }
    public PutSectionProfileRequest[] Body { get; init; }
}
