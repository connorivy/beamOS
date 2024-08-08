namespace BeamOs.Tests.TestRunner;

public class CodeTestScoresTracker(
    SortedList<Version, CodeTestScoreSnapshot> releaseVersionToTestScoreSnapshot
) : ICodeTestScoreTracker
{
    protected SortedList<
        Version,
        CodeTestScoreSnapshot
    > ReleaseVersionToTestScoreSnapshot { get; } = releaseVersionToTestScoreSnapshot;

    public IReadOnlyDictionary<Version, CodeTestScoreSnapshot> CodeTestScores =>
        this.ReleaseVersionToTestScoreSnapshot.AsReadOnly();

    public CodeTestScoreSnapshot LatestScores =>
        this.ReleaseVersionToTestScoreSnapshot.Last().Value;
}
