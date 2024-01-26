using BeamOS.Common.Api.Interfaces;
using BeamOS.Common.Api.Mappers;
using BeamOS.PhysicalModel.Application.SectionProfiles;
using BeamOS.PhysicalModel.Contracts.SectionProfile;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Api.SectionProfiles.Mappers;

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
