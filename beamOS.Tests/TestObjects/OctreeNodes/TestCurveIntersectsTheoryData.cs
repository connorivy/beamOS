namespace beamOS.Tests.TestObjects.OctreeNodes;
using System.Collections.Generic;
using beamOS.API.Schema.Objects;
using OG = Objects.Geometry;

public sealed class TestCurveIntersectsFixture : SerializableFixtureBase
{
  [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes", true)]
  public TestCurveIntersectsFixture()
  {
  }

  public TestCurveIntersectsFixture((double, double, double) center, double size, ICurve curve, bool expectedValue)
  {
    this.Center = new OG.Point(center.Item1, center.Item2, center.Item3);
    this.Size = size;
    this.Curve = curve;
    this.ExpectedValue = expectedValue;
  }

  public OG.Point Center { get; set; }
  public double Size { get; set; }
  public ICurve Curve { get; set; }
  public bool ExpectedValue { get; set; }
}

internal sealed class TestCurveIntersectsTheoryData : TheoryDataBase<TestCurveIntersectsFixture>
{
  public override List<TestCurveIntersectsFixture> AllTestObjects => new()
  {
    Intersects1()
  };

  public static TestCurveIntersectsFixture Intersects1()
  {
    var nodeCenter = (0.0, 0.0, 0.0);
    var nodeSize = 10.0;

    var curve = new Line((0, 0, -100), (0, 0, 100));

    var doesIntersect = true;
    return new TestCurveIntersectsFixture(nodeCenter, nodeSize, curve, doesIntersect);
  }
}
