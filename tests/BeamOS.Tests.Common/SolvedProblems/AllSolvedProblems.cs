using BeamOS.Tests.Common.SolvedProblems.Fixtures;
using BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

namespace BeamOS.Tests.Common.SolvedProblems;

public class AllSolvedProblems : TheoryDataBase<ModelFixture>
{
    public static Kassimali_Example8_4 Kassimali_Examples8_4 { get; } = new();

    public AllSolvedProblems()
    {
        this.Add(Kassimali_Examples8_4);
    }
}
