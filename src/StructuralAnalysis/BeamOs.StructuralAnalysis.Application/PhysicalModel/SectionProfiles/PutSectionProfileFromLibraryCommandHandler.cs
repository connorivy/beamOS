using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;

public class PutSectionProfileFromLibraryCommandHandler(
    ISectionProfileRepository sectionProfileRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<PutSectionProfileFromLibraryCommand, SectionProfileFromLibraryContract>
{
    public async Task<Result<SectionProfileFromLibraryContract>> ExecuteAsync(
        PutSectionProfileFromLibraryCommand command,
        CancellationToken ct = default
    )
    {
        SectionProfile? sectionProfile = SectionProfile.FromLibraryValue(
            command.ModelId,
            command.Library,
            command.Name
        );
        if (sectionProfile is null)
        {
            return BeamOsError.NotFound(
                description: $"Section profile with code {command.Library} not found."
            );
        }
        await sectionProfileRepository.Put(sectionProfile);
        await unitOfWork.SaveChangesAsync(ct);

        return new SectionProfileFromLibraryContract()
        {
            Id = sectionProfile.Id,
            Name = command.Name,
            Library = command.Library,
        };
    }
}

public readonly struct PutSectionProfileFromLibraryCommand
    : IModelResourceWithIntIdRequest<SectionProfileFromLibraryData>
{
    public int Id { get; init; }
    public Guid ModelId { get; init; }
    public SectionProfileFromLibraryData Body { get; init; }
    public StructuralCode Library => this.Body.Library;
    public string Name => this.Body.Name;

    public PutSectionProfileFromLibraryCommand() { }

    [SetsRequiredMembers]
    public PutSectionProfileFromLibraryCommand(
        ModelId modelId,
        SectionProfileFromLibraryContract putSectionProfileFromLibraryRequest
    )
    {
        this.Id = putSectionProfileFromLibraryRequest.Id;
        this.ModelId = modelId;
        this.Body = putSectionProfileFromLibraryRequest;
    }
}

public class BatchPutSectionProfileFromLibraryCommandHandler(
    ISectionProfileFromLibraryRepository sectionProfileFromLibraryRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<
        SectionProfileId,
        SectionProfileFromLibrary,
        BatchPutSectionProfileFromLibraryCommand,
        SectionProfileFromLibraryContract
    >(sectionProfileFromLibraryRepository, unitOfWork)
{
    protected override SectionProfileFromLibrary ToDomainObject(
        ModelId modelId,
        SectionProfileFromLibraryContract putRequest
    ) => new PutSectionProfileFromLibraryCommand(modelId, putRequest).ToDomainObject();
}

public readonly struct BatchPutSectionProfileFromLibraryCommand
    : IModelResourceRequest<SectionProfileFromLibraryContract[]>
{
    public Guid ModelId { get; init; }
    public SectionProfileFromLibraryContract[] Body { get; init; }
}
