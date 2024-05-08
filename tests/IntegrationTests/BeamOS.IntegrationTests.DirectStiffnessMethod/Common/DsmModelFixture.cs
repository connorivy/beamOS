using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeamOs.Domain.AnalyticalResults.Common.ValueObjects;
using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.DirectStiffnessMethod;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;
using Riok.Mapperly.Abstractions;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common;

public abstract class DsmModelFixture : ModelFixture
{
    public DsmModelFixture(ModelFixture fixture)
    {
        this.Fixture = fixture;
        this.StrongModelId = new(Guid.Parse(fixture.ModelId));
    }

    [UseMapper]
    public ModelFixture Fixture { get; }

    public DsmNodeVo ToDsmNodeVo(NodeFixture fixture)
    {
        return new DsmNodeVo(
            new NodeId(Guid.Parse(this.Fixture.FixtureGuidToIdDict[fixture.Id])),
            fixture.LocationPoint,
            fixture.Restraint,
            this.Fixture.PointLoadFixtures.Where(pl => pl.Node == fixture).Select(this.ToDomainObject).ToList(),
            this.Fixture.MomentLoadFixtures.Where(pl => pl.Node == fixture).Select(this.ToDomainObject).ToList()
        );
    }

    public ModelId StrongModelId { get; }

    public Guid LocalGuidToServerGuid(Guid id)
    {
        return Guid.Parse(this.FixtureGuidToIdDict[id]);
    }
    public PointLoad ToDomainObject(PointLoadFixture fixture)
    {
        return new(this.StrongModelId, new(this.LocalGuidToServerGuid(fixture.Node.Id)), fixture.Force, fixture.Direction);
    }

    public MomentLoad ToDomainObject(MomentLoadFixture fixture)
    {
        return new(this.StrongModelId, new(this.LocalGuidToServerGuid(fixture.Node.Id)), fixture.Torque, fixture.AxisDirection.ToVector());
    }

    // public DsmElement1d ToDsm(Element1dFixture fixture)
    // {
    //     return new DsmElement1d(
    //         fixture.SectionProfileRotation,
    //         ToDomainObject(fixture.StartNode),
    //         ToDomainObject(fixture.EndNode),
    //
    //         new NodeId(Guid.Parse(this.Fixture.FixtureGuidToIdDict[fixture.Id])),
    //         fixture.LocationPoint,
    //         fixture.Restraint,
    //         this.Fixture.PointLoadFixtures.Where(pl => pl.Node == fixture).Select(this.ToDomainObject).ToList(),
    //         this.Fixture.MomentLoadFixtures.Where(pl => pl.Node == fixture).Select(this.ToDomainObject).ToList()
    //     );
    // }

    public Node ToDomainObject(NodeFixture fixture)
    {
        return new(this.StrongModelId, fixture.LocationPoint, fixture.Restraint,
            new(this.LocalGuidToServerGuid(fixture.Id)));
    }

    //public Node ToDomainObject(NodeFixture fixture)
    //{
    //    return new(this.StrongModelId, fixture.LocationPoint, fixture.Restraint,
    //        new(this.LocalGuidToServerGuid(fixture.Id)));
    //}
}

public interface IDsmModelFixture
{
    public ModelFixture Fixture { get; }
}

public interface IHasUnsupportedStructureDisplacementIds : IDsmModelFixture
{
    public IEnumerable<(
        NodeFixture node,
        CoordinateSystemDirection3D direction
    )> DegreeOfFreedomIds { get; }

    public IEnumerable<(
        NodeFixture node,
        CoordinateSystemDirection3D direction
    )> BoundaryConditionIds { get; }
}

public static class IHasUnsupportedStructureDisplacementIdsExtensions
{
    public static IEnumerable<UnsupportedStructureDisplacementId2> GetDegreeOfFreedomIds(
        this IHasUnsupportedStructureDisplacementIds dsmFixture
    )
    {
        return GetUnsupportedStructureDisplacementIds(dsmFixture, dsmFixture.DegreeOfFreedomIds);
    }

    public static IEnumerable<UnsupportedStructureDisplacementId2> GetBoundaryConditionIds(
        this IHasUnsupportedStructureDisplacementIds dsmFixture
    )
    {
        return GetUnsupportedStructureDisplacementIds(dsmFixture, dsmFixture.BoundaryConditionIds);
    }

    private static IEnumerable<UnsupportedStructureDisplacementId2> GetUnsupportedStructureDisplacementIds(
        IDsmModelFixture dsmFixture,
        IEnumerable<(NodeFixture, CoordinateSystemDirection3D)> fixtureIds
    )
    {
        foreach (var (node, direction) in fixtureIds)
        {
            yield return new UnsupportedStructureDisplacementId2(
                new NodeId(Guid.Parse(dsmFixture.Fixture.FixtureGuidToIdDict[node.Id])),
                direction
            );
        }
    }
}
