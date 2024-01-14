using BeamOS.PhysicalModel.Domain.ModelAggregate;
using MathNet.Numerics.LinearAlgebra;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures.Models;

public class ModelFixture(Model model)
{
    public Model AnalyticalModel { get; } = model;
    public Matrix<double>? ExpectedStructureStiffnessMatrix { get; set; }
    public Vector<double>? ExpectedDisplacementVector { get; set; }
    public Vector<double>? ExpectedReactionVector { get; set; }
    public int NumberOfDecimalsToCompareSMatrix { get; set; } = 2;
    public int NumberOfDecimalsToCompareDisplacementVector { get; set; } = 2;
    public int NumberOfDecimalsToCompareReactionVector { get; set; } = 2;
}
