using BeamOS.Common.Api;
using BeamOS.Common.Api.Mappers;
using BeamOS.DirectStiffnessMethod.Application.SectionProfiles;
using BeamOS.PhysicalModel.Contracts.SectionProfile;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Api.SectionProfiles.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitValueDtoToAreaMapper))]
[UseStaticMapper(typeof(UnitValueDtoToAreaMomentOfInertiaMapper))]
public partial class SectionProfileResponseToCreateSectionProfileCommand
    : AbstractMapper<SectionProfileResponse, CreateSectionProfileCommand>
{
    public override CreateSectionProfileCommand Map(SectionProfileResponse from) =>
        this.ToCommand(from);

    public partial CreateSectionProfileCommand ToCommand(SectionProfileResponse from);
}
