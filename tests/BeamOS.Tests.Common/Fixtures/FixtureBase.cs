using BeamOs.ApiClient.Builders;

namespace BeamOS.Tests.Common.Fixtures;

public class FixtureBase2 : IHasFixtureId
{
    public virtual FixtureId Id { get; init; } = Guid.NewGuid();
}
