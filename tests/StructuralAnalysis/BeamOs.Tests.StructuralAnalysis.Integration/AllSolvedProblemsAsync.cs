using BeamOs.Tests.Common;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

public static class AllSolvedProblemsAsync
{
    public static async IAsyncEnumerable<ModelFixture> ModelFixtures()
    {
        foreach (var modelBuilder in AllSolvedProblems.ModelFixtures())
        {
            await modelBuilder.Build(AssemblySetup.StructuralAnalysisApiClient);
            await AssemblySetup
                .StructuralAnalysisApiClient
                .RunOpenSeesAnalysisAsync(modelBuilder.Id);
            yield return modelBuilder;
        }
    }
}
