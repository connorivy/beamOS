using BeamOs.StructuralAnalysis;
using BeamOs.Tests.Common;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

public record ApiClientKey
{
    public string Key { get; }

    private ApiClientKey(string key)
    {
        this.Key = key;
    }

    public static ApiClientKey Remote => new("Remote");
    public static ApiClientKey Local => new("Local");

    public BeamOsResultApiClient GetClient()
    {
        return this.Key switch
        {
            "Remote" => AssemblySetup.StructuralAnalysisRemoteApiClient,
            "Local" => AssemblySetup.StructuralAnalysisLocalApiClient,
            _ => throw new NotImplementedException(),
        };
    }
}

public static class AllSolvedProblemsWithMulipleClients
{
    /// <summary>
    /// When you add a new model fixture, please add it to the 'GetModelsQueryHandler' class in BeamOs.Struct..Infrastructure
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<(ApiClientKey, ModelFixture)> ModelFixtures()
    {
        List<ApiClientKey> clients = [ApiClientKey.Remote, ApiClientKey.Local];

        foreach (var client in clients)
        {
            foreach (var fixture in AllSolvedProblems.ModelFixtures())
            {
                yield return (client, fixture);
            }
        }
    }

    public static IEnumerable<(
        ApiClientKey,
        ModelFixture
    )> ModelFixturesWithStructuralStiffnessMatrix()
    {
        return ModelFixtures().Where(x => x.Item2 is IHasStructuralStiffnessMatrix);
    }

    public static IEnumerable<(
        ApiClientKey,
        ModelFixture
    )> ModelFixturesWithExpectedDisplacementVector()
    {
        return ModelFixtures().Where(x => x.Item2 is IHasExpectedDisplacementVector);
    }

    public static IEnumerable<(
        ApiClientKey,
        ModelFixture
    )> ModelFixturesWithExpectedReactionVector()
    {
        return ModelFixtures().Where(x => x.Item2 is IHasExpectedReactionVector);
    }

    public static IEnumerable<(ApiClientKey, ModelFixture)> ModelFixturesWithDsmElement1dResults()
    {
        return ModelFixtures().Where(x => x.Item2 is IHasDsmElement1dResults);
    }

    public static IEnumerable<(ApiClientKey, ModelFixture)> ModelFixturesWithExpectedNodeResults()
    {
        return ModelFixtures().Where(x => x.Item2 is IHasExpectedNodeResults);
    }

    public static IEnumerable<(
        ApiClientKey,
        ModelFixture
    )> DsmModelFixturesWithExpectedNodeResults()
    {
        return ModelFixtures().Where(x => x.Item2 is IHasExpectedNodeResults);
    }

    public static IEnumerable<(
        ApiClientKey,
        ModelFixture
    )> ModelFixturesWithExpectedDiagramResults()
    {
        return ModelFixtures().Where(x => x.Item2 is IHasExpectedDiagramResults);
    }
}

internal static class ApiClients
{
    public static IEnumerable<ApiClientKey> GetClients()
    {
        yield return ApiClientKey.Remote;
        // yield return ApiClientKey.Local;
    }
}
