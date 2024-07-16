using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOS.Tests.Common.Fixtures;

public class FixtureBase
{
    public virtual Guid Id { get; } = Guid.NewGuid();
    public virtual GuidWrapperForModelId ModelId { get; }
}

/// <summary>
/// Having a different type for ModelIds allows mappers on modelFixtures to substitute in their own id if
/// this value has not been overridden
/// </summary>
/// <param name="ModelId"></param>
public record struct GuidWrapperForModelId(Guid? ModelId = null);

public interface IModelMember
{
    public ModelFixture Model { get; }
}

public record FixtureBase2
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
