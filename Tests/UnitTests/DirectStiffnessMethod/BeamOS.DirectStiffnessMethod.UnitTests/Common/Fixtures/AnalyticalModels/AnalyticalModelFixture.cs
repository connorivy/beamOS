using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate;
using MathNet.Numerics.LinearAlgebra;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalModels;

public class AnalyticalModelFixture
{
    public AnalyticalModelFixture(AnalyticalModel model) => this.AnalyticalModel = model;

    public AnalyticalModel AnalyticalModel { get; }
    public Matrix<double>? ExpectedStructureStiffnessMatrix { get; set; }
    public Vector<double>? ExpectedSupportDisplacementVector { get; set; }
    public Vector<double>? ExpectedSupportReactionVector { get; set; }
    public int NumberOfDecimalsToCompareSMatrix { get; set; } = 2;
    public int NumberOfDecimalsToCompareDisplacementVector { get; set; } = 2;
    public int NumberOfDecimalsToCompareReactionVector { get; set; } = 2;
}
