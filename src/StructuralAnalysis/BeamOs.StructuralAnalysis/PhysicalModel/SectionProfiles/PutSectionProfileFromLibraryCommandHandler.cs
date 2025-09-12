using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;

internal class PutSectionProfileFromLibraryCommandHandler(
    ISectionProfileRepository sectionProfileRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : ICommandHandler<
        ModelResourceWithIntIdRequest<SectionProfileFromLibraryData>,
        SectionProfileFromLibraryContract
    >
{
    public async Task<Result<SectionProfileFromLibraryContract>> ExecuteAsync(
        ModelResourceWithIntIdRequest<SectionProfileFromLibraryData> command,
        CancellationToken ct = default
    )
    {
        SectionProfile? sectionProfile = SectionProfile.FromLibraryValue(
            command.ModelId,
            command.Body.Library,
            command.Body.Name
        );
        if (sectionProfile is null)
        {
            return BeamOsError.NotFound(
                description: $"Section profile with code {command.Body.Library} not found."
            );
        }
        await sectionProfileRepository.Put(sectionProfile);
        await unitOfWork.SaveChangesAsync(ct);

        return new SectionProfileFromLibraryContract()
        {
            Id = sectionProfile.Id,
            Name = command.Body.Name,
            Library = command.Body.Library,
        };
    }
}

internal class BatchPutSectionProfileFromLibraryCommandHandler(
    ISectionProfileFromLibraryRepository sectionProfileFromLibraryRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<
        SectionProfileId,
        SectionProfileFromLibrary,
        ModelResourceRequest<SectionProfileFromLibraryContract[]>,
        SectionProfileFromLibraryContract
    >(sectionProfileFromLibraryRepository, unitOfWork)
{
    protected override SectionProfileFromLibrary ToDomainObject(
        ModelId modelId,
        SectionProfileFromLibraryContract putRequest
    ) =>
        new ModelResourceWithIntIdRequest<SectionProfileFromLibraryData>(
            modelId,
            putRequest.Id,
            putRequest
        ).ToDomainObject();
}
