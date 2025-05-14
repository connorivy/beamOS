using BeamOs.Tests.Common;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

public partial class OpenSeesTests
{
    [Before(HookType.Class)]
    public static async Task RunOpenSeesAnalysis()
    {
        if (AssemblySetup.SkipOpenSeesTests)
        {
            return;
        }

        List<Task> analysisTasks = [];
        foreach (var modelBuilder in AllSolvedProblems.ModelFixtures())
        {
            analysisTasks.Add(RunOpenSeesAnalysis(modelBuilder));
        }
        // var analysisTasks = AllSolvedProblems.ModelFixtures().Select(RunOpenSeesAnalysis).ToList();

        try
        {
            await Task.WhenAll(analysisTasks);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static async Task RunOpenSeesAnalysis(
        ModelFixture modelBuilder,
        int loadCombinationId = 1
    )
    {
        await modelBuilder.CreateOnly(AssemblySetup.StructuralAnalysisApiClient);

        var resultSetIdResponse =
            await AssemblySetup.StructuralAnalysisApiClient.RunOpenSeesAnalysisAsync(
                modelBuilder.Id,
                new() { LoadCombinationIds = [loadCombinationId] }
            );

        resultSetIdResponse.ThrowIfError();
    }
}
