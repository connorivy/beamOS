using System.Text;
using System.Text.Json;

namespace BeamOs.Tests.TestRunner;

public sealed class CodeTestScoresTracker : ICodeTestScoreTracker
{
    private readonly SortedList<Version, CodeTestScoreSnapshot> releaseVersionToTestScoreSnapshot;

    private readonly string folderLocation = Path.Combine(
        DirectoryHelper.GetWebAppClientDir(),
        "wwwroot"
    );
    public const string JsonFileName = "codeTestScoresData.json";
    private readonly string fullPath;
    private static readonly JsonSerializerOptions options = new() { WriteIndented = true };

    public CodeTestScoresTracker()
    {
        this.fullPath = Path.Combine(this.folderLocation, JsonFileName);

        //if (!File.Exists(this.fullPath))
        //{
        //    this.releaseVersionToTestScoreSnapshot = [];
        //    this.Save();
        //}

        string jsonString = File.ReadAllText(this.fullPath);
        this.releaseVersionToTestScoreSnapshot =
            JsonSerializer.Deserialize<SortedList<Version, CodeTestScoreSnapshot>>(jsonString)
            ?? throw new Exception("Unable to deserialize releaseVersionToTestScoreSnapshot");
    }

    public IReadOnlyDictionary<Version, CodeTestScoreSnapshot> CodeTestScores =>
        this.releaseVersionToTestScoreSnapshot.AsReadOnly();

    public CodeTestScoreSnapshot LatestScores =>
        this.releaseVersionToTestScoreSnapshot.Last().Value;

    internal void AddScore(Version release, CodeTestScoreSnapshot scoreSnapshot)
    {
        this.releaseVersionToTestScoreSnapshot[release] = scoreSnapshot;
    }

    internal void Save()
    {
        string json =
            JsonSerializer.Serialize(this.releaseVersionToTestScoreSnapshot, options)
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

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"NumTests: {this.NumTests}");
        sb.AppendLine($"CodeCov%: {this.CodeCoveragePercent}");
        sb.AppendLine($"MutationScore: {this.MutationScore}");

        return sb.ToString();
    }
}
