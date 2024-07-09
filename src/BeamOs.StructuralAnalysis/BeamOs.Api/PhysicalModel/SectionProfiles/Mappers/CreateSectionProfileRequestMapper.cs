using BeamOs.Api.Common.Mappers;
using BeamOs.Application.PhysicalModel.SectionProfiles;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.PhysicalModel.SectionProfile;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.SectionProfiles.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitValueDtoToAreaMapper))]
[UseStaticMapper(typeof(UnitValueDtoToAreaMomentOfInertiaMapper))]
public partial class CreateSectionProfileRequestMapper
    : IMapper<CreateSectionProfileRequest, CreateSectionProfileCommand>
{
    public CreateSectionProfileCommand Map(CreateSectionProfileRequest from) =>
        this.ToCommand(from);

    public partial CreateSectionProfileCommand ToCommand(CreateSectionProfileRequest request);
}
