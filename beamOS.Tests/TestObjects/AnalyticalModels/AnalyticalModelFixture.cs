using beamOS.API.Schema.Objects;
using beamOS.Tests.TestObjects.Element1Ds;
using LanguageExt;
using MathNet.Numerics.LinearAlgebra;

namespace beamOS.Tests.TestObjects.AnalyticalModels
{
  public class AnalyticalModelFixture : SerializableFixtureBase<AnalyticalModel>, IHasGlobalResults
  {
    public AnalyticalModelFixture() { }
    public AnalyticalModelFixture(AnalyticalModel model)
    {
      AnalyticalModel = model;
    }
    public AnalyticalModel AnalyticalModel { get; set; }
    public Option<Matrix<double>> ExpectedGlobalStiffnessMatrix { get; set; }
    public Option<Vector<double>> ExpectedGlobalFixedEndForces { get; set; }
    public Option<Vector<double>> ExpectedGlobalEndDisplacements { get; set; }
    public Option<Vector<double>> ExpectedGlobalEndForces { get; set; }
  }
}
