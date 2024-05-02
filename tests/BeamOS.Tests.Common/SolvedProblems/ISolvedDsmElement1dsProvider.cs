using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod;
using MathNet.Numerics.LinearAlgebra;

namespace BeamOS.Tests.Common.SolvedProblemFixtures;

public interface ISolvedDsmElement1dsProvider
{
    public IEnumerable<SolvedDsmElement1dFixture> GetSolvedDsmElement1Ds();
}

public class SolvedDsmElement1dFixture
{
    public SolvedDsmElement1dFixture(DsmElement1d element, UnitSettings unitSettings)
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
