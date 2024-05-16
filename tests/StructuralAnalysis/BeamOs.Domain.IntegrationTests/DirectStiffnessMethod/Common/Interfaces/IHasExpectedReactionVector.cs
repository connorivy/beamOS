namespace BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Interfaces;

public interface IHasExpectedReactionVector : IDsmModelFixture
{
    public double[] ExpectedReactionVector { get; }
}
