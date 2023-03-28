using beamOS.Tests.TestObjects.Element1Ds;
using beamOS.Tests.TestObjects.SolvedProblems.MatrixAnalysisOfStructures_2ndEd;
using Xunit.Abstractions;

namespace beamOS.Tests.TestObjects
{
  public class SerializableFixtureBaseTests
  {
    [Fact]
    public void TestSerialize()
    {
      var solvedProblem = new Example8_4();
      var info = new info();
      var fixture = solvedProblem.Element1DFixtures.First();
      fixture.Serialize(info);

      var newFixture = new Element1DFixture();
      newFixture.Deserialize(info);
    }

    public class info : IXunitSerializationInfo
    {
      Dictionary<string, object> _dictionary = new();
      public void AddValue(string key, object value, Type type = null)
      {
        _dictionary[key] = value;
      }

      public object GetValue(string key, Type type)
      {
        _dictionary.TryGetValue(key, out var result);
        return result;
      }

      public T GetValue<T>(string key)
      {
        if (_dictionary.TryGetValue(key, out var result))
          return (T)result;
        return default;
      }
    }
  }
}
