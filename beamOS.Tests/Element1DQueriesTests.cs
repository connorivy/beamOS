using beamOS.API.Schema.Queries;

namespace beamOS.Tests;

public class Element1DQueriesTests
{
  [Fact]
  public void Test1()
  {
    Assert.Equal(5, Element1DQueries.GetBeamLength());
  }
}