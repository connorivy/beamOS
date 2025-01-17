using BeamOs.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd;

namespace BeamOs.Tests.Common;

public static class AllSolvedProblems
{
    public static IEnumerable<ModelFixture> ModelFixtures()
    {
        yield return new Kassimali_Example3_8();
        yield return new Kassimali_Example8_4();
    }
}

public static class AllSolvedProblemsFilter<T>
{
    public static IEnumerable<T> ModelFixtures()
    {
        return AllSolvedProblems.ModelFixtures().OfType<T>();
    }
}
