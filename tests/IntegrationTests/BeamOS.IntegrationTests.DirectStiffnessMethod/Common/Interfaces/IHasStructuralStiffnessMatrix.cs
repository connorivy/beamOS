namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Interfaces;

public interface IHasStructuralStiffnessMatrix : IDsmModelFixture
{
    public double[,] ExpectedStructuralStiffnessMatrix { get; }
}
