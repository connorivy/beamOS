using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;

public class PutSectionProfileFromLibraryCommandHandler(
    ISectionProfileRepository sectionProfileRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : ICommandHandler<
        PutSectionProfileFromLibraryCommand,
        Contracts.PhysicalModel.SectionProfile.SectionProfileFromLibrary
    >
{
    public async Task<
        Result<Contracts.PhysicalModel.SectionProfile.SectionProfileFromLibrary>
    > ExecuteAsync(PutSectionProfileFromLibraryCommand command, CancellationToken ct = default)
    {
        SectionProfile? sectionProfile = SectionProfile.FromLibraryValue(
            command.ModelId,
            command.StructuralCode,
            command.Name
        );
        if (sectionProfile is null)
        {
            return BeamOsError.NotFound(
                description: $"Section profile with code {command.StructuralCode} not found."
            );
        }
        sectionProfileRepository.Put(sectionProfile);
        await unitOfWork.SaveChangesAsync(ct);

        return new Contracts.PhysicalModel.SectionProfile.SectionProfileFromLibrary()
        {
            Id = sectionProfile.Id,
            Name = command.Name,
            Library = command.StructuralCode,
        };
    }
}

public readonly struct PutSectionProfileFromLibraryCommand
    : IModelResourceWithIntIdRequest<StructuralCodeSectionProfileData>
{
    public int Id { get; init; }
    public Guid ModelId { get; init; }
    public StructuralCodeSectionProfileData Body { get; init; }
    public StructuralCode StructuralCode => this.Body.Library;
    public string Name => this.Body.Name;

    public PutSectionProfileFromLibraryCommand() { }

    [SetsRequiredMembers]
    public PutSectionProfileFromLibraryCommand(
        ModelId modelId,
        Contracts.PhysicalModel.SectionProfile.SectionProfileFromLibrary putSectionProfileFromLibraryRequest
    )
    {
        this.Id = putSectionProfileFromLibraryRequest.Id;
        this.ModelId = modelId;
        this.Body = putSectionProfileFromLibraryRequest;
    }
}
