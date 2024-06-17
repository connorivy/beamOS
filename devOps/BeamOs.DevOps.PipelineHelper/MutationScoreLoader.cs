using System.Text.Json;

namespace BeamOs.DevOps.PipelineHelper;

public class MutationScoreLoader
{
    private readonly string folderLocation;
    private readonly string fileName = "mutationScoreData.json";
    private readonly string fullPath;
    private readonly SortedList<Version, float> releaseVersionToMutationScores;

    public MutationScoreLoader()
    {
        this.folderLocation = typeof(MutationScoreLoader).Assembly.Location;
        this.fullPath = Path.Combine(this.folderLocation, this.fileName);

        string jsonString = File.ReadAllText(this.fullPath);
        this.releaseVersionToMutationScores =
            JsonSerializer.Deserialize<SortedList<Version, float>>(jsonString)
            ?? throw new Exception("Unable to deserialize releaseVersionToMutationScores");
    }

    public IReadOnlyDictionary<Version, float> MutationScores =>
        this.releaseVersionToMutationScores.AsReadOnly();

    internal void AddScore(Version release, float score)
    {
        this.releaseVersionToMutationScores.Add(release, score);
    }

    internal void Save()
    {
        string json =
            JsonSerializer.Serialize(this.releaseVersionToMutationScores)
            ?? throw new Exception("Serialized value was null");

        File.WriteAllText(this.fullPath, json);
    }
}
