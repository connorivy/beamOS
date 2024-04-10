using BeamOs.IntegrationTests.StructuralAnalysis.Diagrams.Fixtures;

namespace BeamOs.IntegrationTests.StructuralAnalysis.Diagrams;

public class UnitTest1
{
    [SkippableTheory]
    [ClassData(typeof(AllSolvedDiagramProblems))]
    public void ShearDiagram_ForAllSolvedDiagramFixtures_ShouldEvaluateToExpectedValues(
        SolvedDiagramProblem solvedDiagramProblem
    )
    {
        var shearDiagram = solvedDiagramProblem.ShearDiagram ?? throw new SkipException();

        foreach (var (location, value) in solvedDiagramProblem.ExpectedShearDiagramValues)
        {
            var diagramValue = shearDiagram.GetValueAtLocation(location);
            Assert.Equal(value, diagramValue);
        }
    }

    [SkippableTheory]
    [ClassData(typeof(AllSolvedDiagramProblems))]
    public void MomentDiagram_ForAllSolvedDiagramFixtures_ShouldEvaluateToExpectedValues(
        SolvedDiagramProblem solvedDiagramProblem
    )
    {
        var momentDiagram = solvedDiagramProblem.MomentDiagram ?? throw new SkipException();

        foreach (var (location, value) in solvedDiagramProblem.ExpectedMomentDiagramValues)
        {
            var diagramValue = momentDiagram.GetValueAtLocation(location);
            Assert.Equal(value, diagramValue);
        }
    }
}
