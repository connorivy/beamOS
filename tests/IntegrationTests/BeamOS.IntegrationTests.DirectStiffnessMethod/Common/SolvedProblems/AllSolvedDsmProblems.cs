using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;
using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.SolvedProblems.MatrixAnalysisOfStructures2ndEd.Example8_4;

namespace BeamOS.Tests.Common.SolvedProblems;

internal class AllSolvedDsmProblems : TheoryDataBase<DsmModelFixture>
{
    public static Example8_4_Dsm Example8_4_Dsm { get; } =
        new(AllSolvedProblems.Kassimali_Examples8_4);
    public static AllSolvedDsmProblems Instance { get; } = new();

    private AllSolvedDsmProblems()
    {
        this.Add(Example8_4_Dsm);
    }
}
