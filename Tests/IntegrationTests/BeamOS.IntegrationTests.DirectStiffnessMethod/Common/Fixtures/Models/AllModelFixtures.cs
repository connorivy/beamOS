using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures.SolvedProblems;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures.Models;

public class AllModelFixtures : TheoryData<ModelFixture>
{
    public List<ModelFixture> AnalyticalModelFixtures { get; } =

        [
        // add standalone fixtures here
        ];

    public AllModelFixtures()
    {
        var allSolvedProblems = new AllSolvedProblems();
        foreach (var solvedProblem in allSolvedProblems.GetItems())
        {
            this.Add(solvedProblem.ModelFixture);
        }

        foreach (var analyticalModelFixture in this.AnalyticalModelFixtures)
        {
            this.Add(analyticalModelFixture);
        }
    }
}
