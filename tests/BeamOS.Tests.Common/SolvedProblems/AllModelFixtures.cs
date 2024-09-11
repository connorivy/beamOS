using BeamOs.ApiClient.Builders;
using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.SolvedProblems.ETABS_Models.Simple_3_Story_Diagonal;
using BeamOS.Tests.Common.SolvedProblems.ETABS_Models.TwistyBowlFraming;
using BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;
using BeamOS.Tests.Common.SolvedProblems.Udoeyo_StructuralAnalysis.Example7_11;

namespace BeamOS.Tests.Common.SolvedProblems;

public class AllModelFixtures : TheoryDataBase<ModelFixture2>
{
    public AllModelFixtures()
    {
        this.Add(Kassimali_Example8_4.Instance);
        this.Add(Udoeyo_StructuralAnalysis_Example7_11.Instance);
    }
}

public class AllCreateModelRequestBuilders : TheoryDataBase<CreateModelRequestBuilder>
{
    public AllCreateModelRequestBuilders()
    {
        this.Add(TwistyBowlFraming.Instance);
        this.Add(Simple_3_Story_Diagonal.Instance);
    }
}

public class AllSolvedProblemsFilter<T> : TheoryDataBase<T>
{
    public AllSolvedProblemsFilter()
    {
        AllModelFixtures allSolved = new();
        foreach (T item in allSolved.GetItems().OfType<T>())
        {
            this.Add(item);
        }
    }
}

public class AllCreateModelRequestBuildersFilter<T> : TheoryDataBase<T>
{
    public AllCreateModelRequestBuildersFilter()
    {
        AllCreateModelRequestBuilders allSolved = new();
        foreach (T item in allSolved.GetItems().OfType<T>())
        {
            this.Add(item);
        }
    }
}
