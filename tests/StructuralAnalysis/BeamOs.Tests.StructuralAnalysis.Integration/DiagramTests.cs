using BeamOs.Tests.Common;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

internal class DiagramTests
{
    [Test]
    [MethodDataSource(
        typeof(AllSolvedProblems),
        nameof(AllSolvedProblems.ModelFixturesWithExpectedDiagramResults)
    )]
    public async Task ShearDiagrams_ShouldHaveCorrectMaxAndMinValue(ModelFixture modelFixture)
    {
        var diagramResults = await AssemblySetup
            .StructuralAnalysisApiClient
            .GetResultSetAsync(modelFixture.Id, 1);
        diagramResults.ThrowIfError();

        var expectedDiagramResults = (
            (IHasExpectedDiagramResults)modelFixture
        ).ExpectedDiagramResults;
        //var shearResultDict = diagramResults.Value.e.ToDictionary(x => x.ElementId);
        //if (expectedDiagramResults.MaxShear != null)
        //{
        //    Asserter.AssertEqual(BeamOsObjectType.Element1d,)
        //}
    }
}
