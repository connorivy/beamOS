using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.SolvedProblems;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalElement1Ds;
public class AllElement1DFixtures : TheoryData<AnalyticalElement1DFixture>
{
    public List<AnalyticalElement1DFixture> Element1DFixtures { get; } = new()
    {
        // add standalone element 1D fixtures here
    };
    public AllElement1DFixtures()
    {
        var allSolvedProblems = new AllSolvedProblems();
        foreach (var solvedProblem in allSolvedProblems.GetItems())
        {
            foreach (var element1DFixture in solvedProblem.Element1DFixtures)
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
