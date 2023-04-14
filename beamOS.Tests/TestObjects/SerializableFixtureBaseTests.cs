namespace beamOS.Tests.TestObjects;
using beamOS.Tests.TestObjects.AnalyticalModels;
using beamOS.Tests.TestObjects.Element1Ds;
using beamOS.Tests.TestObjects.SolvedProblems.MatrixAnalysisOfStructures_2ndEd;
using Xunit.Abstractions;

public class SerializableFixtureBaseTests
{
  [Fact]
  public void TestSerializeElement1DFixture()
  {
    var solvedProblem = new Example8_4();
    var info = new info();
    var fixture = solvedProblem.Element1DFixtures.First();
    fixture.Serialize(info);

    var newFixture = new Element1DFixture();
    newFixture.Deserialize(info);
  }

  [Fact]
  public void TestSerializeAnalyticalModelFixture()
  {
    var solvedProblem = new Example8_4();
    var info = new info();
    var fixture = solvedProblem.AnalyticalModelFixture;
    var centerCoords = new double[] { 1, 1, 1 };
    fixture.ExpectedOctreeCenter = centerCoords;
    fixture.Serialize(info);

    var newFixture = new AnalyticalModelFixture();
    newFixture.Deserialize(info);
    _ = newFixture.ExpectedOctreeCenter.Match(
      center => Assert.Equal(center, centerCoords),
      () => throw new Exception("This value should be some")
    );
  }

  public class info : IXunitSerializationInfo
  {
    private readonly Dictionary<string, object> _dictionary = new();
    public void AddValue(string key, object value, Type? type = null) => this._dictionary[key] = value;

    public object GetValue(string key, Type type)
    {
      _ = this._dictionary.TryGetValue(key, out var result);
      return result;
    }

    public T GetValue<T>(string key)
    {
      if (this._dictionary.TryGetValue(key, out var result))
      {
        return (T)result;
      }

      return default;
    }
  }
}
