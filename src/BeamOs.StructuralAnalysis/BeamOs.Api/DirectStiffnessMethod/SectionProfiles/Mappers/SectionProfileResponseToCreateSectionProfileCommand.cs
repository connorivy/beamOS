using BeamOs.Api.Common.Mappers;
using BeamOs.Application.Common.Models;
using BeamOs.Application.DirectStiffnessMethod.SectionProfiles;
using BeamOs.Contracts.PhysicalModel.SectionProfile;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.DirectStiffnessMethod.SectionProfiles.Mappers;

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
