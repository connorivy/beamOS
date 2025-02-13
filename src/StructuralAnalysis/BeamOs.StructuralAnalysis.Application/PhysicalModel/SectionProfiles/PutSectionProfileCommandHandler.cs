using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;

public class PutSectionProfileCommandHandler(
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
        sectionProfileRepository.Add(sectionProfile);
        await unitOfWork.SaveChangesAsync(ct);

        return sectionProfile.ToResponse();
    }
}

public class BatchPutSectionProfileCommandHandler(
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
public static partial class PutSectionProfileCommandMapper
{
    public static partial SectionProfile ToDomainObject(this PutSectionProfileCommand command);
}

public readonly struct PutSectionProfileCommand
    : IModelResourceWithIntIdRequest<SectionProfileRequestData>
{
    public int Id { get; init; }
    public Guid ModelId { get; init; }
    public SectionProfileRequestData Body { get; init; }
    public AreaContract Area => this.Body.Area;
    public AreaMomentOfInertiaContract StrongAxisMomentOfInertia =>
        this.Body.StrongAxisMomentOfInertia;
    public AreaMomentOfInertiaContract WeakAxisMomentOfInertia => this.Body.WeakAxisMomentOfInertia;
    public AreaMomentOfInertiaContract PolarMomentOfInertia => this.Body.PolarMomentOfInertia;
    public AreaContract StrongAxisShearArea => this.Body.StrongAxisShearArea;
    public AreaContract WeakAxisShearArea => this.Body.WeakAxisShearArea;

    public PutSectionProfileCommand() { }

    public PutSectionProfileCommand(
        ModelId modelId,
        PutSectionProfileRequest putSectionProfileRequest
    )
    {
        this.Id = putSectionProfileRequest.Id;
        this.ModelId = modelId;
        this.Body = putSectionProfileRequest;
    }
}

public readonly struct BatchPutSectionProfileCommand
    : IModelResourceRequest<PutSectionProfileRequest[]>
{
    public Guid ModelId { get; init; }
    public PutSectionProfileRequest[] Body { get; init; }
}
