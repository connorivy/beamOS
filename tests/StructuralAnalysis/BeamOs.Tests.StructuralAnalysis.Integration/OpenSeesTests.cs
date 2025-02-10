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

        foreach (var modelBuilder in AllSolvedProblems.ModelFixtures())
        {
            await modelBuilder.Build(AssemblySetup.StructuralAnalysisApiClient);

            await AssemblySetup
                .StructuralAnalysisApiClient
                .DeleteAllResultSetsAsync(modelBuilder.Id);

            var resultSetIdResponse = await AssemblySetup
                .StructuralAnalysisApiClient
                .RunOpenSeesAnalysisAsync(modelBuilder.Id);

            if (resultSetIdResponse.IsError)
            {
                throw new Exception(resultSetIdResponse.Error.Description);
            }
        }
    }
}
