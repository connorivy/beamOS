using BeamOs.Api.Common.Mappers;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Material;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.MomentLoad;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOs.Contracts.PhysicalModel.SectionProfile;
using BeamOS.Tests.Common.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

public partial class ModelFixtureInDb
{
    public CreateModelRequest ToRequest(ModelFixtureInDb fixture)
    {
        return new(
            fixture.ModelFixture.Name,
            fixture.ModelFixture.Description,
            new PhysicalModelSettingsDto(fixture.ModelFixture.UnitSettings.ToContract()),
            fixture.ModelFixture.Id.ToString()
        );
    }

    public partial CreateElement1dRequest ToRequest(Element1dFixture fixture);

    public partial CreatePointLoadRequest ToRequest(PointLoadFixture fixture);

    public partial CreateMomentLoadRequest ToRequest(MomentLoadFixture fixture);

    public partial CreateMaterialRequest ToRequest(MaterialFixture fixture);

    public partial CreateSectionProfileRequest ToRequest(SectionProfileFixture fixture);

    public partial CreateNodeRequest ToRequest(NodeFixture fixture);
}
