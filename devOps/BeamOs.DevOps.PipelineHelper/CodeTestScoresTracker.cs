using System.Text.Json;

namespace BeamOs.DevOps.PipelineHelper;

public sealed class CodeTestScoresTracker
{
    private readonly SortedList<Version, CodeTestScoreSnapshot> releaseVersionToTestScoreSnapshot;

    private readonly string folderLocation = DirectoryHelper.GetPipelineHelperDir();
    private readonly string fileName = "codeTestScoresData.json";
    private readonly string fullPath;

    public CodeTestScoresTracker()
    {
        this.fullPath = Path.Combine(this.folderLocation, this.fileName);

        string jsonString = File.ReadAllText(this.fullPath);
        this.releaseVersionToTestScoreSnapshot =
            JsonSerializer.Deserialize<SortedList<Version, CodeTestScoreSnapshot>>(jsonString)
            ?? throw new Exception("Unable to deserialize releaseVersionToTestScoreSnapshot");
    }

    public IReadOnlyDictionary<Version, CodeTestScoreSnapshot> CodeTestScores =>
        this.releaseVersionToTestScoreSnapshot.AsReadOnly();

    internal void AddScore(Version release, CodeTestScoreSnapshot scoreSnapshot)
    {
        this.releaseVersionToTestScoreSnapshot.Add(release, scoreSnapshot);
    }

    internal void Save()
    {
        string json =
            JsonSerializer.Serialize(this.releaseVersionToTestScoreSnapshot)
            ?? throw new Exception("Serialized value was null");

        File.WriteAllText(this.fullPath, json);
    }
}

public sealed class CodeTestScoreSnapshot(
    int numTests,
    float codeCoveragePercent,
    float mutationScore
)
{
    public int NumTests { get; } = numTests;
    public float CodeCoveragePercent { get; } = codeCoveragePercent;
    public float MutationScore { get; } = mutationScore;
}
