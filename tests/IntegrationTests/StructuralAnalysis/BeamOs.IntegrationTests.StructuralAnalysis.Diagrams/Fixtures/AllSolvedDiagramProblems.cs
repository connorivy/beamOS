using BeamOs.IntegrationTests.StructuralAnalysis.Diagrams.Fixtures.Hibbeler_StructuralAnalysis9thEd;
using BeamOs.IntegrationTests.StructuralAnalysis.Diagrams.Fixtures.Udoeyo_StructuralAnalysis;
using BeamOS.Tests.Common;

namespace BeamOs.IntegrationTests.StructuralAnalysis.Diagrams.Fixtures;

internal class AllSolvedDiagramProblems : TheoryDataBase<ISolvedDiagramProblem>
{
    public static List<ISolvedDiagramProblem> SolvedDiagramProblems { get; private set; }
    public static TheoryDataBase<ISolvedShearDiagramProblem> SolvedShearDiagramProblems { get; }
    public static TheoryDataBase<ISolvedMomentDiagramProblem> SolvedMomentDiagramProblems { get; }
    public static TheoryDataBase<ISolvedRotationDiagramProblem> SolvedRotationDiagramProblems { get; }
    public static TheoryDataBase<ISolvedDeflectionDiagramProblem> SolvedDeflectionDiagramProblems { get; }

    static AllSolvedDiagramProblems()
    {
        SolvedDiagramProblems =  [new Example4_11(), new Example7_6()];

        SolvedShearDiagramProblems = new(
            SolvedDiagramProblems.OfType<ISolvedShearDiagramProblem>()
        );
        SolvedMomentDiagramProblems = new(
            SolvedDiagramProblems.OfType<ISolvedMomentDiagramProblem>()
        );
        SolvedRotationDiagramProblems = new(
            SolvedDiagramProblems.OfType<ISolvedRotationDiagramProblem>()
        );
        SolvedDeflectionDiagramProblems = new(
            SolvedDiagramProblems.OfType<ISolvedDeflectionDiagramProblem>()
        );
    }
}
