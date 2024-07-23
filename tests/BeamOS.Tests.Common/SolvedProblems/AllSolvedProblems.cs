using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;
using BeamOS.Tests.Common.SolvedProblems.Udoeyo_StructuralAnalysis.Example7_11;

namespace BeamOS.Tests.Common.SolvedProblems;

public class AllSolvedProblems : TheoryDataBase<ModelFixture2>
{
    public AllSolvedProblems()
    {
        this.Add(Kassimali_Example8_4.Instance);
        this.Add(Udoeyo_StructuralAnalysis_Example7_11.Instance);
    }
}

public class AllSolvedProblemsFilter<T> : TheoryDataBase<T>
{
    public AllSolvedProblemsFilter()
    {
        AllSolvedProblems allSolved = new();
        foreach (T item in allSolved.GetItems().OfType<T>())
        {
            this.Add(item);
        }
    }
}
