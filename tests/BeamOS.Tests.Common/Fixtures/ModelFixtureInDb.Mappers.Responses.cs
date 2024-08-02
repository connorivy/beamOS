using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Material;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.MomentLoad;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOs.Contracts.PhysicalModel.SectionProfile;
using BeamOs.Domain.Common.ValueObjects;
using BeamOS.Tests.Common.Fixtures;
using Riok.Mapperly.Abstractions;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

[UseStaticMapper(typeof(UnitsNetMappers))]
public partial class ModelFixtureInDb
{
    public partial NodeResponse ToResponse(NodeFixture2 fixture);

    public partial Element1DResponse ToResponse(Element1dFixture2 fixture);

    public partial MaterialResponse ToResponse(MaterialFixture2 fixture);

    public partial SectionProfileResponse ToResponse(SectionProfileFixture2 fixture);

    public partial PointLoadResponse ToResponse(PointLoadFixture2 fixture);

    public partial MomentLoadResponse ToResponse(MomentLoadFixture2 fixture);

    public partial PointResponse ToResponse(BeamOs.Domain.Common.ValueObjects.Point source);

    public partial RestraintResponse ToResponse(Restraint source);

    public partial ModelResponse ToResponse(ModelFixture2 fixture);

    public ModelResponse ToResponse() => this.ToResponse(this.ModelFixture);
}
