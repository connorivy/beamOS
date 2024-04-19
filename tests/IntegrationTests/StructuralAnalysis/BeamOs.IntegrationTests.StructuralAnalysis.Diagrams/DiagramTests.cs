using BeamOs.IntegrationTests.StructuralAnalysis.Diagrams.Fixtures;

namespace BeamOs.IntegrationTests.StructuralAnalysis.Diagrams;

public class DiagramTests
{
    [Theory]
    [MemberData(
        nameof(AllSolvedDiagramProblems.SolvedShearDiagramProblems),
        MemberType = typeof(AllSolvedDiagramProblems)
    )]
    public void ShearDiagram_ForAllSolvedDiagramFixtures_ShouldEvaluateToExpectedValues(
        ISolvedShearDiagramProblem solvedDiagramProblem
    )
    {
        var shearDiagram = solvedDiagramProblem.GetShearDiagram();

        foreach (var (location, value) in solvedDiagramProblem.ExpectedShearDiagramValues)
        {
            var diagramValue = shearDiagram.GetValueAtLocation(location);
            Assert.Equal(value, diagramValue);
        }
    }

    [Theory]
    [MemberData(
        nameof(AllSolvedDiagramProblems.SolvedMomentDiagramProblems),
        MemberType = typeof(AllSolvedDiagramProblems)
    )]
    public void MomentDiagram_ForAllSolvedDiagramFixtures_ShouldEvaluateToExpectedValues(
        ISolvedMomentDiagramProblem solvedDiagramProblem
    )
    {
        var momentDiagram = solvedDiagramProblem.GetMomentDiagram();

        foreach (var (location, value) in solvedDiagramProblem.ExpectedMomentDiagramValues)
        {
            var diagramValue = momentDiagram.GetValueAtLocation(location);
            Assert.Equal(value, diagramValue);
        }
    }

    [Theory]
    [MemberData(
        nameof(AllSolvedDiagramProblems.SolvedDeflectionDiagramProblems),
        MemberType = typeof(AllSolvedDiagramProblems)
    )]
    public void DeflectionDiagram_ForAllSolvedDiagramFixtures_ShouldEvaluateToExpectedValues(
        ISolvedDeflectionDiagramProblem solvedDiagramProblem
    )
    {
        var deflectionDiagram = solvedDiagramProblem.GetDeflectionDiagram();

        foreach (var (location, value) in solvedDiagramProblem.ExpectedDeflectionDiagramValues)
        {
            var diagramValue = deflectionDiagram.GetValueAtLocation(location);
            var epsilon = value - diagramValue;
            Assert.True(
                Math.Abs(epsilon.ValueOnLeft) < 1e-5 && Math.Abs(epsilon.ValueOnRight) < 1e-5
            );
        }
    }
}
