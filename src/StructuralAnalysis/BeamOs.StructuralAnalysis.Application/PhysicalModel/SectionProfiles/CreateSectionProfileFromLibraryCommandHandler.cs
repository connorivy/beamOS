using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;

public class CreateSectionProfileFromLibraryCommandHandler(
    IModelRepository modelRepository,
    ISectionProfileFromLibraryRepository sectionProfileFromLibraryRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceRequest<SectionProfileFromLibraryData>, SectionProfileResponse>
{
    public async Task<Result<SectionProfileResponse>> ExecuteAsync(
        ModelResourceRequest<SectionProfileFromLibraryData> command,
        CancellationToken ct = default
    )
    {
        Model? model = await modelRepository.GetSingle(command.ModelId, ct: ct);
        if (model is null)
        {
            return BeamOsError.NotFound(description: $"Model with id {command.ModelId} not found.");
        }

        SectionProfile? sectionProfile = SectionProfile.FromLibraryValue(
            command.ModelId,
            command.Body.Library,
            command.Body.Name
        );
        if (sectionProfile is null)
        {
            return BeamOsError.NotFound(
                description: $"Section profile named {command.Body.Name} with code {command.Body.Library} not found."
            );
        }

        SectionProfileFromLibrary sectionProfileFromLibrary = command.ToDomainObject();
        sectionProfileFromLibraryRepository.Add(sectionProfileFromLibrary);
        await unitOfWork.SaveChangesAsync(ct);

        return sectionProfile
            .Copy(sectionProfileFromLibrary.Id.Id)
            .ToResponse(model.Settings.UnitSettings.LengthUnit);
    }
}
