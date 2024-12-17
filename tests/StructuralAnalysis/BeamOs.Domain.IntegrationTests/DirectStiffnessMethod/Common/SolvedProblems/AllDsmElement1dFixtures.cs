using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;
using BeamOS.Tests.Common;

namespace BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.SolvedProblems;

public class AllDsmElement1dFixtures : TheoryDataBase<DsmElement1dFixture>
{
    public AllDsmElement1dFixtures()
    {
        foreach (var modelFixture in AllSolvedDsmProblems.Instance.GetItems())
        {
            foreach (var elementId in modelFixture.DsmElement1dFixtures)
            {
                this.Add(elementId);
            }
        }
    }
}
