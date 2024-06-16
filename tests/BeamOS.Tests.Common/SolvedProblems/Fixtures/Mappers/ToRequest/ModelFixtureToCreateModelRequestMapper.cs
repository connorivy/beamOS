using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Domain.Common.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures.Mappers;

[Mapper]
internal static partial class ModelFixtureToCreateModelRequestMapper
{
    public static CreateModelRequest ToRequest(this ModelFixture fixture)
    {
        return new(
            fixture.Name,
            fixture.Description,
            new PhysicalModelSettingsDto(ToDto(fixture.UnitSettings)),
            fixture.Id.ToString()
        );
    }

    private static partial UnitSettingsDtoVerbose ToDto(UnitSettings settings);
}
