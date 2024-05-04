using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod;
using MathNet.Numerics.LinearAlgebra;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalElement1Ds;

public class AnalyticalElement1DFixture : IHasLocalResults, IHasGlobalResults
{
    public AnalyticalElement1DFixture(DsmElement1d element, UnitSettings unitSettings)
    {
        this.Element = element;
        this.UnitSettings = unitSettings;
    }

    public DsmElement1d Element { get; set; }
    public UnitSettings UnitSettings { get; set; }
    public Matrix<double>? ExpectedRotationMatrix { get; set; }
    public Matrix<double>? ExpectedTransformationMatrix { get; set; }
    public Matrix<double>? ExpectedLocalStiffnessMatrix { get; set; }
    public Matrix<double>? ExpectedGlobalStiffnessMatrix { get; set; }
    public Vector<double>? ExpectedLocalFixedEndForces { get; set; }
    public Vector<double>? ExpectedGlobalFixedEndForces { get; set; }
    public Vector<double>? ExpectedLocalEndDisplacements { get; set; }
    public Vector<double>? ExpectedGlobalEndDisplacements { get; set; }
    public Vector<double>? ExpectedLocalEndForces { get; set; }
    public Vector<double>? ExpectedGlobalEndForces { get; set; }
}

public interface IHasLocalResults
{
    public Matrix<double>? ExpectedRotationMatrix { get; set; }
    public Matrix<double>? ExpectedTransformationMatrix { get; set; }
    public Matrix<double>? ExpectedLocalStiffnessMatrix { get; set; }
    public Vector<double>? ExpectedLocalFixedEndForces { get; set; }
    public Vector<double>? ExpectedLocalEndDisplacements { get; set; }
    public Vector<double>? ExpectedLocalEndForces { get; set; }
}

public interface IHasGlobalResults
{
    public Matrix<double>? ExpectedGlobalStiffnessMatrix { get; set; }
    public Vector<double>? ExpectedGlobalFixedEndForces { get; set; }
    public Vector<double>? ExpectedGlobalEndDisplacements { get; set; }
    public Vector<double>? ExpectedGlobalEndForces { get; set; }
}
