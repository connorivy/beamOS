using BeamOs.Domain.DirectStiffnessMethod.AnalyticalModelAggregate;
using MathNet.Numerics.LinearAlgebra;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalModels;

public class AnalyticalModelFixture
{
    public AnalyticalModelFixture(AnalyticalModel model) => this.AnalyticalModel = model;

    public AnalyticalModel AnalyticalModel { get; }
    public Matrix<double>? ExpectedStructureStiffnessMatrix { get; set; }
    public Vector<double>? ExpectedDisplacementVector { get; set; }
    public Vector<double>? ExpectedReactionVector { get; set; }
    public int NumberOfDecimalsToCompareSMatrix { get; set; } = 2;
    public int NumberOfDecimalsToCompareDisplacementVector { get; set; } = 2;
    public int NumberOfDecimalsToCompareReactionVector { get; set; } = 2;
}
