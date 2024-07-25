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
    public CreateModelRequest ToRequest()
    {
        return new(
            this.ModelFixture.Name,
            this.ModelFixture.Description,
            new PhysicalModelSettingsDto(this.ModelFixture.Settings.UnitSettings.ToContract()),
            this.ModelFixture.Id.ToString()
        );
    }

    public partial CreateElement1dRequest ToRequest(Element1dFixture2 fixture);

    public partial CreatePointLoadRequest ToRequest(PointLoadFixture2 fixture);

    public partial CreateMomentLoadRequest ToRequest(MomentLoadFixture2 fixture);

    public partial CreateMaterialRequest ToRequest(MaterialFixture2 fixture);

    public partial CreateSectionProfileRequest ToRequest(SectionProfileFixture2 fixture);

    public partial CreateNodeRequest ToRequest(NodeFixture2 fixture);
}
