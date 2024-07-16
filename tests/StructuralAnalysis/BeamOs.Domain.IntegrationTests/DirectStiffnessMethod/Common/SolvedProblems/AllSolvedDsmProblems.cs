using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.SolvedProblems.MatrixAnalysisOfStructures2ndEd.Example8_4;
using BeamOS.Tests.Common;

namespace BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.SolvedProblems;

public class AllSolvedDsmProblems : TheoryDataBase<DsmModelFixture>
{
    public static AllSolvedDsmProblems Instance { get; } = new();

    public AllSolvedDsmProblems()
    {
        this.Add(Example8_4_Dsm.Instance);
    }
}
