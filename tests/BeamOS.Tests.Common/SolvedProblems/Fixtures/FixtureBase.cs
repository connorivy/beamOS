namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

public class FixtureBase
{
    public Guid Id { get; } = Guid.NewGuid();
    public DummyModelId ModelId { get; }
}
