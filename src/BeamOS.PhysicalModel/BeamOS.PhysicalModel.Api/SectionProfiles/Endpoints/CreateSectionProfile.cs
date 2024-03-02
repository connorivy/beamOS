using BeamOS.Common.Api;
using BeamOS.PhysicalModel.Api.Mappers;
using BeamOS.PhysicalModel.Api.SectionProfiles.Mappers;
using BeamOS.PhysicalModel.Application.SectionProfiles;
using BeamOS.PhysicalModel.Contracts.SectionProfile;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate;

namespace BeamOS.PhysicalModel.Api.SectionProfiles.Endpoints;

public class CreateSectionProfile(
    CreateSectionProfileRequestMapper commandMapper,
    CreateSectionProfileCommandHandler createSectionProfileCommandHandler,
    SectionProfileResponseMapper sectionProfileResponseMapper
) : BeamOsFastEndpoint<CreateSectionProfileRequest, SectionProfileResponse>
{
    public override void Configure()
    {
        this.Post("SectionProfiles");
        this.AllowAnonymous();
    }

    //public override string Route => "SectionProfiles";

    //public override EndpointType EndpointType => EndpointType.Post;

    public override async Task<SectionProfileResponse> ExecuteAsync(
        CreateSectionProfileRequest request,
        CancellationToken ct
    )
    {
        CreateSectionProfileCommand command = commandMapper.Map(request);

        SectionProfile sectionProfile = await createSectionProfileCommandHandler.ExecuteAsync(
            command,
            ct
        );

        SectionProfileResponse response = sectionProfileResponseMapper.Map(sectionProfile);
        return response;
    }
}
