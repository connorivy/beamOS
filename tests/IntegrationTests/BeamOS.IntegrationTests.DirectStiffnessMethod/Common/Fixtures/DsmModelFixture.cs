using BeamOs.Domain.DirectStiffnessMethod;
using BeamOS.IntegrationTests.DirectStiffnessMethod.Common;
using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Interfaces;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;

public abstract class DsmModelFixture : IDsmModelFixture
{
    public DsmModelFixture(ModelFixture fixture)
    {
        this.Fixture = fixture;
    }

    public ModelFixture Fixture { get; }
    public abstract DsmElement1dFixture[] DsmElement1dFixtures { get; }
    public abstract DsmNodeFixture[] DsmNodeFixtures { get; }

    public DsmNodeVo ToDsm(NodeFixture fixture)
    {
        return new DsmNodeVo(
            new(fixture.Id),
            fixture.LocationPoint,
            fixture.Restraint,
            this.Fixture
                .PointLoadFixtures
                .Where(pl => pl.Node == fixture)
                .Select(this.Fixture.ToDomainObjectWithLocalIds)
                .ToList(),
            this.Fixture
                .MomentLoadFixtures
                .Where(pl => pl.Node == fixture)
                .Select(this.Fixture.ToDomainObjectWithLocalIds)
                .ToList()
        );
    }

    public DsmElement1d ToDsm(Element1dFixture fixture)
    {
        return new(
            fixture.SectionProfileRotation,
            this.Fixture.ToDomainObjectWithLocalIds(fixture.StartNode),
            this.Fixture.ToDomainObjectWithLocalIds(fixture.EndNode),
            this.Fixture.ToDomainObjectWithLocalIds(fixture.Material),
            this.Fixture.ToDomainObjectWithLocalIds(fixture.SectionProfile)
        );
    }

    public DsmElement1d ToDsm(DsmElement1dFixture fixture)
    {
        return this.ToDsm(fixture.Fixture);
    }

    public DsmNodeVo ToDsm(DsmNodeFixture fixture)
    {
        return this.ToDsm(fixture.Fixture);
    }
}
