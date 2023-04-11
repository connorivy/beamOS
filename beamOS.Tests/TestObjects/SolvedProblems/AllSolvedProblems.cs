namespace beamOS.Tests.TestObjects.SolvedProblems;

public class AllSolvedProblems : TheoryDataBase<SolvedProblem>
{
  public override List<SolvedProblem> AllTestObjects => new()
  {
    new MatrixAnalysisOfStructures_2ndEd.Example8_4()
  };
}
