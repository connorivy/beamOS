using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Tests.Common;
using BeamOs.Tests.StructuralAnalysis.Integration;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Tests.Runtime.TestRunner;

public class TestInfoRetriever(
    [FromKeyedServices("InMemory")] IStructuralAnalysisApiClientV1 inMemoryApiClient
)
{
    public IEnumerable<TestInfoBase> GetTestInfos()
    {
        var tests = new ModelRepairerTests(inMemoryApiClient);
        yield return new ModelRepairTestInfo<ModelRepairerTests>(
            static async (testClass) =>
                await testClass.ProposeRepairs_MergesCloseNodes_AddsNodeProposal(),
            nameof(ModelRepairerTests.ProposeRepairs_MergesCloseNodes_AddsNodeProposal),
            tests,
            inMemoryApiClient
        );
        yield return new ModelRepairTestInfo<ModelRepairerTests>(
            static async (testClass) => await testClass.NodesVeryCloseToColumn_ShouldSnapToColumn(),
            nameof(ModelRepairerTests.NodesVeryCloseToColumn_ShouldSnapToColumn),
            tests,
            inMemoryApiClient
        );
        yield return new ModelRepairTestInfo<ModelRepairerTests>(
            static async (testClass) =>
                await testClass.NearlyConvergingNodesInXYPlane_ShouldMergeOrSnapNodes(),
            nameof(ModelRepairerTests.NearlyConvergingNodesInXYPlane_ShouldMergeOrSnapNodes),
            tests,
            inMemoryApiClient
        );
        yield return new ModelRepairTestInfo<ModelRepairerTests>(
            static async (testClass) =>
                await testClass.ColumnWithNearbyBeam_ShouldSnapBeamNodeToColumn(),
            nameof(ModelRepairerTests.ColumnWithNearbyBeam_ShouldSnapBeamNodeToColumn),
            tests,
            inMemoryApiClient
        );
        foreach (var modelFixture in AllSolvedProblems.ModelFixturesWithExpectedNodeResults())
        {
            var openSeesTest = new OpenSeesTests(modelFixture);
            var response = modelFixture.MapToResponse();
            yield return new StructuralAnalysisTestInfo<OpenSeesTests>(
                static async (testClass) =>
                    await testClass.AssertNodeResults_AreApproxEqualToExpectedValues(),
                modelFixture.Name,
                openSeesTest,
                response
            );
        }
    }
}
