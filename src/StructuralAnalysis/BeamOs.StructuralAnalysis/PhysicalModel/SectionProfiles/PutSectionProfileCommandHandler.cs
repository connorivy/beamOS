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
) : ICommandHandler<ModelResourceWithIntIdRequest<SectionProfileData>, SectionProfileResponse>
{
    public async Task<Result<SectionProfileResponse>> ExecuteAsync(
        ModelResourceWithIntIdRequest<SectionProfileData> command,
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
        ModelResourceRequest<PutSectionProfileRequest[]>,
        PutSectionProfileRequest
    >(sectionProfileRepository, unitOfWork)
{
    protected override SectionProfile ToDomainObject(
        ModelId modelId,
        PutSectionProfileRequest putRequest
    ) =>
        new ModelResourceWithIntIdRequest<SectionProfileData>(
            modelId,
            putRequest.Id,
            putRequest
        ).ToDomainObject();
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

    [MapNestedProperties(nameof(ModelResourceWithIntIdRequest<>.Body))]
    public static partial SectionProfileFromLibrary ToDomainObject(
        this ModelResourceWithIntIdRequest<SectionProfileFromLibraryData> command
    );
}
