using BeamOS.Common.Api.Interfaces;
using BeamOS.PhysicalModel.Contracts.SectionProfile;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Api.Mappers;

[Mapper]
public partial class SectionProfileResponseMapper : IMapper<SectionProfile, SectionProfileResponse>
{
    public SectionProfileResponse Map(SectionProfile from) => this.ToResponse(from);
    public partial SectionProfileResponse ToResponse(SectionProfile model);
}
