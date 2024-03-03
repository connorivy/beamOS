using BeamOs.Api.Common.Interfaces;
using BeamOs.Api.Common.Mappers;
using BeamOS.PhysicalModel.Application.SectionProfiles;
using BeamOS.PhysicalModel.Contracts.SectionProfile;
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
