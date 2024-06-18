namespace BeamOs.Tests.TestRunner;

public sealed class CodeTestScoresTrackerWasm(
    SortedList<Version, CodeTestScoreSnapshot> releaseVersionToTestScoreSnapshot
) : ICodeTestScoreTracker
{
    public IReadOnlyDictionary<Version, CodeTestScoreSnapshot> CodeTestScores =>
        releaseVersionToTestScoreSnapshot.AsReadOnly();

    public CodeTestScoreSnapshot LatestScores => releaseVersionToTestScoreSnapshot.Last().Value;
}
