namespace beamOS.Tests.TestObjects.AnalyticalModels;
using beamOS.API.Schema.Objects;
using beamOS.Tests.TestObjects.Element1Ds;
using MathNet.Numerics.LinearAlgebra;

public class AnalyticalModelFixture : SerializableFixtureBase, IHasGlobalResults
{
  public AnalyticalModelFixture() { }
  public AnalyticalModelFixture(AnalyticalModel model) => this.AnalyticalModel = model;
  public AnalyticalModel AnalyticalModel { get; set; }
  public Matrix<double>? ExpectedGlobalStiffnessMatrix { get; set; }
  public Vector<double>? ExpectedGlobalFixedEndForces { get; set; }
  public Vector<double>? ExpectedGlobalEndDisplacements { get; set; }
  public Vector<double>? ExpectedGlobalEndForces { get; set; }
  public double[]? ExpectedOctreeCenter { get; set; }
}
