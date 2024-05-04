using BeamOs.Application.Common.Mappers;
using BeamOs.Contracts.PhysicalModel.SectionProfile;
using BeamOs.Infrastructure.Data.Models;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.SectionProfiles.Mappers;

[Mapper]
internal partial class SectionProfileReadModelToResponseMapper
    : AbstractMapper<SectionProfileReadModel, SectionProfileResponse>
{
    public override SectionProfileResponse Map(SectionProfileReadModel source) =>
        this.ToResponse(source);

    public partial SectionProfileResponse ToResponse(SectionProfileReadModel source);
}
