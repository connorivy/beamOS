namespace BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Interfaces;

public interface IHasExpectedDisplacementVector : IDsmModelFixture
{
    public double[] ExpectedDisplacementVector { get; }
}
