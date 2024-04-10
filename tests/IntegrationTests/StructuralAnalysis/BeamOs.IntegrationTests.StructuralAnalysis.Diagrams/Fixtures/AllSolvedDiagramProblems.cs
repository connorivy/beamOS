using BeamOs.IntegrationTests.StructuralAnalysis.Diagrams.Fixtures.Hibbeler_StructuralAnalysis9thEd;

namespace BeamOs.IntegrationTests.StructuralAnalysis.Diagrams.Fixtures;

internal class AllSolvedDiagramProblems : TheoryData<SolvedDiagramProblem>
{
    public AllSolvedDiagramProblems()
    {
        this.Add(new Example4_11());
    }
}
