using BeamOs.Api.Common.Mappers;
using BeamOs.Contracts.PhysicalModel.Material;
using BeamOS.Tests.Common.Fixtures;

namespace BeamOS.Tests.Common.Fixtures.Mappers.ToRequest;

internal static partial class MaterialFixtureToCreateNodeRequestMapper
{
    public static CreateMaterialRequest ToRequest(this MaterialFixture fixture, string modelId)
    {
        return new(modelId, fixture.ModulusOfElasticity.ToDto(), fixture.ModulusOfRigidity.ToDto());
    }
}
