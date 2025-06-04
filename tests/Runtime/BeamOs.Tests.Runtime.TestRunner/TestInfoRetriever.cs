using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Tests.Common;
using BeamOs.Tests.StructuralAnalysis.Integration;
using BeamOs.Tests.StructuralAnalysis.Integration.ModelRepairerTests;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Tests.Runtime.TestRunner;

public class TestInfoRetriever(
    [FromKeyedServices("InMemory")] IStructuralAnalysisApiClientV1 inMemoryApiClient
)
{
    public IEnumerable<TestInfoBase> GetTestInfos()
    {
        var tests = new EnvelopeModelRepairerTests(inMemoryApiClient);
        yield return new ModelRepairTestInfo<EnvelopeModelRepairerTests>(
            static async (testClass) =>
                await testClass.ProposeRepairs_MergesCloseNodes_AddsNodeProposal(),
            nameof(EnvelopeModelRepairerTests.ProposeRepairs_MergesCloseNodes_AddsNodeProposal),
            tests,
            inMemoryApiClient
        );
        yield return new ModelRepairTestInfo<EnvelopeModelRepairerTests>(
            static async (testClass) => await testClass.NodesVeryCloseToColumn_ShouldSnapToColumn(),
            nameof(EnvelopeModelRepairerTests.NodesVeryCloseToColumn_ShouldSnapToColumn),
            tests,
            inMemoryApiClient
        );
        yield return new ModelRepairTestInfo<EnvelopeModelRepairerTests>(
            static async (testClass) =>
                await testClass.NearlyConvergingNodesInXYPlane_ShouldMergeOrSnapNodes(),
            nameof(
                EnvelopeModelRepairerTests.NearlyConvergingNodesInXYPlane_ShouldMergeOrSnapNodes
            ),
            tests,
            inMemoryApiClient
        );
        yield return new ModelRepairTestInfo<EnvelopeModelRepairerTests>(
            static async (testClass) =>
                await testClass.ColumnWithNearbyBeam_ShouldSnapBeamNodeToColumn(),
            nameof(EnvelopeModelRepairerTests.ColumnWithNearbyBeam_ShouldSnapBeamNodeToColumn),
            tests,
            inMemoryApiClient
        );
        yield return new ModelRepairTestInfo<EnvelopeModelRepairerTests>(
            static async (testClass) =>
                await testClass.BraceBetweenTwoColumns_ButSlightlyOutOfPlane_ShouldSnapIntoPlane(),
            nameof(
                EnvelopeModelRepairerTests.BraceBetweenTwoColumns_ButSlightlyOutOfPlane_ShouldSnapIntoPlane
            ),
            tests,
            inMemoryApiClient
        );
        yield return new ModelRepairTestInfo<EnvelopeModelRepairerTests>(
            static async (testClass) =>
                await testClass.BraceBetweenTwoColumns_ButSlightlyOutOfPlane_ShouldSnapIntoPlane2(),
            nameof(
                EnvelopeModelRepairerTests.BraceBetweenTwoColumns_ButSlightlyOutOfPlane_ShouldSnapIntoPlane
            ),
            tests,
            inMemoryApiClient
        );

        var extendElement1dToNodeRuleTest = new ExtendElement1dToNodeRuleTests(inMemoryApiClient);
        yield return new ModelRepairTestInfo<ExtendElement1dToNodeRuleTests>(
            static async (testClass) => await testClass.PerpendicularBeams_ShouldExtendToMeet(),
            nameof(ExtendElement1dToNodeRuleTests.PerpendicularBeams_ShouldExtendToMeet),
            extendElement1dToNodeRuleTest,
            inMemoryApiClient
        );
        yield return new ModelRepairTestInfo<ExtendElement1dToNodeRuleTests>(
            static async (testClass) =>
                await testClass.BeamAtAngleFromOther_ShouldExtendToMeetOtherNode(),
            nameof(ExtendElement1dToNodeRuleTests.BeamAtAngleFromOther_ShouldExtendToMeetOtherNode),
            extendElement1dToNodeRuleTest,
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
