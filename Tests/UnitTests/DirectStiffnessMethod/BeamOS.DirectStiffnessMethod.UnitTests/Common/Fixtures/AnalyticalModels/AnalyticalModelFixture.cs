using BeamOS.DirectStiffnessMethod.Domain.ModelAggregate;
using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalElement1Ds;
using MathNet.Numerics.LinearAlgebra;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalModels;
public class AnalyticalModelFixture : IHasGlobalResults
{
    public AnalyticalModelFixture() { }
    public AnalyticalModelFixture(Model model) => this.AnalyticalModel = model;
    public Model AnalyticalModel { get; set; }
    public Matrix<double>? ExpectedGlobalStiffnessMatrix { get; set; }
    public Vector<double>? ExpectedGlobalFixedEndForces { get; set; }
    public Vector<double>? ExpectedGlobalEndDisplacements { get; set; }
    public Vector<double>? ExpectedGlobalEndForces { get; set; }
    public double[]? ExpectedOctreeCenter { get; set; }
}
