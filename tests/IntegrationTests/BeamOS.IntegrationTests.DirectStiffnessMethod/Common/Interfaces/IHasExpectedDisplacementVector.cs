namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Interfaces;

public interface IHasExpectedDisplacementVector : IDsmModelFixture
{
    public double[] ExpectedDisplacementVector { get; }
}
