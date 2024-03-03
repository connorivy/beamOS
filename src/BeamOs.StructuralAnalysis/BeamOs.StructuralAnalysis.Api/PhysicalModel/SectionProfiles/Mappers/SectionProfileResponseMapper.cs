using BeamOs.Api.Common.Interfaces;
using BeamOS.PhysicalModel.Contracts.SectionProfile;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.SectionProfiles.Mappers;

[Mapper]
public partial class SectionProfileResponseMapper : IMapper<SectionProfile, SectionProfileResponse>
{
    public SectionProfileResponse Map(SectionProfile from) => this.ToResponse(from);

    public partial SectionProfileResponse ToResponse(SectionProfile model);
}
