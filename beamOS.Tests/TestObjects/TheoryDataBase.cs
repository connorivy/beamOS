namespace beamOS.Tests.TestObjects;

public abstract class TheoryDataBase<T> : TheoryData<T>
  where T : class
{
  public abstract List<T> AllTestObjects { get; }
  public TheoryDataBase()
  {
    foreach (var solvedProblem in this.AllTestObjects)
    {
      this.Add(solvedProblem);
    }
  }
}
