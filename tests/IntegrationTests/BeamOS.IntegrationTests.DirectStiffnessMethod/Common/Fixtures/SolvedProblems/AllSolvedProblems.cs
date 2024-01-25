namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures.SolvedProblems;

public class AllSolvedProblems : TheoryData<SolvedProblem>
{
    public AllSolvedProblems()
    {
        //this.Add(new MatrixAnalysisOfStructures2ndEd.Example3_8());
        this.Add(new MatrixAnalysisOfStructures2ndEd.Example8_4());
    }

    public IEnumerable<SolvedProblem> GetItems()
    {
        foreach (var item in this)
        {
            yield return CastObjectsToReturnType(item);
        }
    }

    private static SolvedProblem CastObjectsToReturnType(object[]? objects)
    {
        if (objects == null || objects.Length != 1 || objects[0] is not SolvedProblem solved)
        {
            throw new ArgumentNullException(nameof(objects));
        }
        return solved;
    }
}
