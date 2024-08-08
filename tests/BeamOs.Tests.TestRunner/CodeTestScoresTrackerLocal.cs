using System.Text;
using System.Text.Json;

namespace BeamOs.Tests.TestRunner;

public sealed class CodeTestScoresTrackerLocal : CodeTestScoresTracker
{
    private static readonly string FolderLocation = DirectoryHelper.GetServerWwwrootDir();
    public const string JsonFileName = "codeTestScoresData.json";

    private static readonly string FullPath = Path.Combine(FolderLocation, JsonFileName);
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public CodeTestScoresTrackerLocal()
        : base(ReadTestScores()) { }

    private static SortedList<Version, CodeTestScoreSnapshot> ReadTestScores()
    {
        //if (!File.Exists(this.fullPath))
        //{
        //    this.releaseVersionToTestScoreSnapshot = [];
        //    this.Save();
        //}

        string jsonString = File.ReadAllText(FullPath);
        return JsonSerializer.Deserialize<SortedList<Version, CodeTestScoreSnapshot>>(jsonString)
            ?? throw new Exception("Unable to deserialize releaseVersionToTestScoreSnapshot");
    }

    internal void AddScore(Version release, CodeTestScoreSnapshot scoreSnapshot)
    {
        this.ReleaseVersionToTestScoreSnapshot[release] = scoreSnapshot;
    }

    internal void Save()
    {
        string json =
            JsonSerializer.Serialize(this.ReleaseVersionToTestScoreSnapshot, Options)
            ?? throw new Exception("Serialized value was null");

        File.WriteAllText(FullPath, json);
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
