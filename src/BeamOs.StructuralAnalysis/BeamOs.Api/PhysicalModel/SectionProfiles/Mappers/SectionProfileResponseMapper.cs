using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.PhysicalModel.SectionProfile;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.SectionProfiles.Mappers;

[Mapper]
public partial class SectionProfileResponseMapper : IMapper<SectionProfile, SectionProfileResponse>
{
    public SectionProfileResponse Map(SectionProfile from) => this.ToResponse(from);

    public partial SectionProfileResponse ToResponse(SectionProfile model);
}
