using BeamOS.Tests.Common.SolvedProblems.Fixtures;
using BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

namespace BeamOS.Tests.Common.SolvedProblems;

public class AllSolvedProblems : TheoryDataBase<ModelFixture2>
{
    public AllSolvedProblems()
    {
        this.Add(Kassimali_Example8_4.Instance);
    }
}
