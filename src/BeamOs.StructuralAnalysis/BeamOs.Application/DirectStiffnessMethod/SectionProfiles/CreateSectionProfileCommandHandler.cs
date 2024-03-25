using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.DirectStiffnessMethod.SectionProfiles;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Application.SectionProfiles;

public class CreateSectionProfileCommandHandler
    : ICommandHandler<CreateSectionProfileCommand, SectionProfile>
{
    public Task<SectionProfile> ExecuteAsync(
        CreateSectionProfileCommand command,
        CancellationToken ct
    )
    {
        SectionProfile sectionProfile = command.ToDomainObject();

        return Task.FromResult(sectionProfile);
    }
}

[Mapper]
public static partial class CreateSectionProfileCommandMapper
{
    public static partial SectionProfile ToDomainObject(this CreateSectionProfileCommand command);
}
