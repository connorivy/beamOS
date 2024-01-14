using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using MathNet.Numerics.LinearAlgebra;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures.Element1ds;

public class Element1dFixture(Element1D element, UnitSettings unitSettings)
    : IHasLocalResults,
        IHasGlobalResults
{
    public Element1D Element { get; } = element;
    public UnitSettings UnitSettings { get; } = unitSettings;
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
