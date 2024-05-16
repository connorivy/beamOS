using BeamOs.Api.Common.Mappers;
using BeamOs.Contracts.PhysicalModel.Material;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures.Mappers.ToRequest;

internal static partial class MaterialFixtureToCreateNodeRequestMapper
{
    public static CreateMaterialRequest ToRequest(this MaterialFixture fixture, string modelId)
    {
        return new(modelId, fixture.ModulusOfElasticity.ToDto(), fixture.ModulusOfRigidity.ToDto());
    }
}
