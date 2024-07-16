using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Material;
using BeamOs.Contracts.PhysicalModel.MomentLoad;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOs.Contracts.PhysicalModel.SectionProfile;
using BeamOs.Domain.Common.ValueObjects;
using BeamOS.Tests.Common.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

public partial class ModelFixture
{
    public partial NodeResponse ToResponse(NodeFixture fixture);

    public partial Element1DResponse ToResponse(Element1dFixture fixture);

    public partial MaterialResponse ToResponse(MaterialFixture fixture);

    public partial SectionProfileResponse ToResponse(SectionProfileFixture fixture);

    public partial PointLoadResponse ToResponse(PointLoadFixture fixture);

    public partial MomentLoadResponse ToResponse(MomentLoadFixture fixture);

    public partial PointResponse ToResponse(BeamOs.Domain.Common.ValueObjects.Point source);

    public partial RestraintResponse ToResponse(Restraint source);
}
