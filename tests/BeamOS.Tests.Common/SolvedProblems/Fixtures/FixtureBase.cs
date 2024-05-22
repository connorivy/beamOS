namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

public class FixtureBase
{
    public virtual Guid Id { get; } = Guid.NewGuid();
    public DummyModelId ModelId { get; }
}
