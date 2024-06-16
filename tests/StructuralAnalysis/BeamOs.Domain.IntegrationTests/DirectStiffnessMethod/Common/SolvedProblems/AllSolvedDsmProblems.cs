using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.SolvedProblems.MatrixAnalysisOfStructures2ndEd.Example8_4;
using BeamOS.Tests.Common;
using BeamOS.Tests.Common.SolvedProblems;

namespace BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.SolvedProblems;

public class AllSolvedDsmProblems : TheoryDataBase<DsmModelFixture>
{
    public static Example8_4_Dsm Example8_4_Dsm { get; } =
        new(AllSolvedProblems.Kassimali_Examples8_4);
    public static AllSolvedDsmProblems Instance { get; } = new();

    public AllSolvedDsmProblems()
    {
        this.Add(Example8_4_Dsm);
    }
}
