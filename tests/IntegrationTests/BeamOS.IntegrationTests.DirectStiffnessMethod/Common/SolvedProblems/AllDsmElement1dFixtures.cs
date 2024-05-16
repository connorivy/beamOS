using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;
using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.SolvedProblems.MatrixAnalysisOfStructures2ndEd.Example8_4;

namespace BeamOS.Tests.Common.SolvedProblems;

public class AllDsmElement1dFixtures : TheoryDataBase<DsmElement1dFixture>
{
    public static Example8_4_Dsm Example8_4_Dsm { get; } =
        new(AllSolvedProblems.Kassimali_Examples8_4);

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
