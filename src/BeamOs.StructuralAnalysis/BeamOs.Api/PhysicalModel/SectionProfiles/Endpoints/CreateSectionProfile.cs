using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Api.PhysicalModel.SectionProfiles.Mappers;
using BeamOs.Application.PhysicalModel.SectionProfiles;
using BeamOs.Contracts.PhysicalModel.SectionProfile;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.SectionProfiles.Endpoints;

public class CreateSectionProfile(
    BeamOsFastEndpointOptions options,
    CreateSectionProfileCommandHandler createSectionProfileCommandHandler,
    SectionProfileResponseMapper sectionProfileResponseMapper
) : BeamOsFastEndpoint<CreateSectionProfileRequest, SectionProfileResponse>(options)
{
    public override Http EndpointType => Http.POST;

    public override string Route => "SectionProfiles";

    public override async Task<SectionProfileResponse> ExecuteAsync(
        CreateSectionProfileRequest request,
        CancellationToken ct
    )
    {
        SectionProfile sectionProfile = await createSectionProfileCommandHandler.ExecuteAsync(
            request,
            ct
        );

        var response = sectionProfileResponseMapper.Map(sectionProfile);
        return response;
    }
}
