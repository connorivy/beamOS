using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.SectionProfiles;

public class CreateSectionProfileCommandHandler(
    IRepository<SectionProfileId, SectionProfile> sectionProfileRepository
) : ICommandHandler<CreateSectionProfileCommand, SectionProfile>
{
    public async Task<SectionProfile> ExecuteAsync(
        CreateSectionProfileCommand command,
        CancellationToken ct
    )
    {
        SectionProfile load = command.ToDomainObject();

        await sectionProfileRepository.Add(load);

        return load;
    }
}

[Mapper]
public static partial class CreateSectionProfileCommandMapper
{
    public static partial SectionProfile ToDomainObject(this CreateSectionProfileCommand command);
}
