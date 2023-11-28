using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate;
using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalElement1Ds;
using MathNet.Numerics.LinearAlgebra;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalModels;
public class AnalyticalModelFixture : IHasGlobalResults
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
