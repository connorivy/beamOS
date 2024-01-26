using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures.SolvedProblems;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures.Element1ds;

public class AllElement1dFixtures : TheoryData<Element1dFixture>
{
    public List<Element1dFixture> Element1DFixtures { get; } = [];

    public AllElement1dFixtures()
    {
        var allSolvedProblems = new AllSolvedProblems();
        foreach (var solvedProblem in allSolvedProblems.GetItems())
        {
            foreach (var element1DFixture in solvedProblem.Element1dFixtures)
            {
                this.Add(element1DFixture);
            }
        }

        foreach (var element1DFixture in this.Element1DFixtures)
        {
            this.Add(element1DFixture);
        }
    }
}
