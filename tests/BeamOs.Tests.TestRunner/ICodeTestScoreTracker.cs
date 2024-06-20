namespace BeamOs.Tests.TestRunner;

public interface ICodeTestScoreTracker
{
    public IReadOnlyDictionary<Version, CodeTestScoreSnapshot> CodeTestScores { get; }

    public CodeTestScoreSnapshot LatestScores { get; }
}
