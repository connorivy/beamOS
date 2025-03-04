using BeamOs.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd;
using BeamOs.Tests.Common.SolvedProblems.SAP2000;
using BeamOs.Tests.Common.SolvedProblems.Udoeyo_StructuralAnalysis;

namespace BeamOs.Tests.Common;

public static class AllSolvedProblems
{
    public static IEnumerable<ModelFixture> ModelFixtures()
    {
        yield return new Kassimali_Example3_8();
        yield return new Kassimali_Example8_4();
        yield return new Udoeyo_Example7_7();
        yield return new TwistyBowlFraming();
    }

    public static IEnumerable<ModelFixture> ModelFixturesWithStructuralStiffnessMatrix()
    {
        return ModelFixtures().Where(x => x is IHasStructuralStiffnessMatrix);
    }

    public static IEnumerable<ModelFixture> ModelFixturesWithExpectedDisplacementVector()
    {
        return ModelFixtures().Where(x => x is IHasExpectedDisplacementVector);
    }

    public static IEnumerable<ModelFixture> ModelFixturesWithExpectedReactionVector()
    {
        return ModelFixtures().Where(x => x is IHasExpectedReactionVector);
    }

    public static IEnumerable<ModelFixture> ModelFixturesWithDsmElement1dResults()
    {
        return ModelFixtures().Where(x => x is IHasDsmElement1dResults);
    }

    public static IEnumerable<ModelFixture> ModelFixturesWithExpectedNodeResults()
    {
        return ModelFixtures().Where(x => x is IHasExpectedNodeResults);
    }

    public static IEnumerable<ModelFixture> ModelFixturesWithExpectedDiagramResults()
    {
        return ModelFixtures().Where(x => x is IHasExpectedDiagramResults);
    }
}

public static class AllSolvedProblemsFilter<T>
{
    public static IEnumerable<T> ModelFixtures()
    {
        return AllSolvedProblems.ModelFixtures().OfType<T>();
    }
}
