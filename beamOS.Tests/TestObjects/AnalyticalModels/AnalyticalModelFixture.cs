namespace beamOS.Tests.TestObjects.AnalyticalModels;
using beamOS.API.Schema.Objects;
using beamOS.Tests.TestObjects.Element1Ds;
using LanguageExt;
using MathNet.Numerics.LinearAlgebra;

public class AnalyticalModelFixture : SerializableFixtureBase, IHasGlobalResults
{
  public AnalyticalModelFixture() { }
  public AnalyticalModelFixture(AnalyticalModel model) => this.AnalyticalModel = model;
  public AnalyticalModel AnalyticalModel { get; set; }
  public Option<Matrix<double>> ExpectedGlobalStiffnessMatrix { get; set; }
  public Option<Vector<double>> ExpectedGlobalFixedEndForces { get; set; }
  public Option<Vector<double>> ExpectedGlobalEndDisplacements { get; set; }
  public Option<Vector<double>> ExpectedGlobalEndForces { get; set; }
  public Option<double[]> ExpectedOctreeCenter { get; set; }
}
