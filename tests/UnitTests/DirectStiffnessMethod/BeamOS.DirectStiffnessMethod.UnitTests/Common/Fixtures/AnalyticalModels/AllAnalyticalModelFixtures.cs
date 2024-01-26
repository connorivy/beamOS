using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.SolvedProblems;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalModels;

public class AllAnalyticalModelFixtures : TheoryData<AnalyticalModelFixture>
{
    public List<AnalyticalModelFixture> AnalyticalModelFixtures { get; } =

        [
        // add standalone fixtures here
        ];

    public AllAnalyticalModelFixtures()
    {
        var allSolvedProblems = new AllSolvedProblems();
        foreach (var solvedProblem in allSolvedProblems.GetItems())
        {
            this.Add(solvedProblem.AnalyticalModelFixture);
        }

        foreach (var analyticalModelFixture in this.AnalyticalModelFixtures)
        {
            this.Add(analyticalModelFixture);
        }
    }
}
