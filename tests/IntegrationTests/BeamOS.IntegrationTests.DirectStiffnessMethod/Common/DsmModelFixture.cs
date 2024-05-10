using BeamOs.Domain.AnalyticalResults.Common.ValueObjects;
using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.DirectStiffnessMethod;
using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;
using MathNet.Numerics.LinearAlgebra;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common;

public abstract class DsmModelFixture
{
    public DsmModelFixture(ModelFixture fixture)
    {
        this.Fixture = fixture;
    }

    public ModelFixture Fixture { get; }

    public DsmNodeVo ToDsm(NodeFixture fixture)
    {
        return new DsmNodeVo(
            this.Fixture.ToNodeId(fixture.Id),
            fixture.LocationPoint,
            fixture.Restraint,
            this.Fixture
                .PointLoadFixtures
                .Where(pl => pl.Node == fixture)
                .Select(this.Fixture.ToDomainObject)
                .ToList(),
            this.Fixture
                .MomentLoadFixtures
                .Where(pl => pl.Node == fixture)
                .Select(this.Fixture.ToDomainObject)
                .ToList()
        );
    }

    public DsmElement1d ToDsm(Element1dFixture fixture)
    {
        return new(
            fixture.SectionProfileRotation,
            this.Fixture.ToDomainObject(fixture.StartNode),
            this.Fixture.ToDomainObject(fixture.EndNode),
            this.Fixture.ToDomainObject(fixture.Material),
            this.Fixture.ToDomainObject(fixture.SectionProfile)
        );
    }

    public DsmElement1d ToDsm(DsmElement1dFixture fixture)
    {
        return this.ToDsm(fixture.Fixture);
    }
}

public interface IDsmModelFixture
{
    public ModelFixture Fixture { get; }
    public DsmElement1dFixture[] DsmElement1ds2 { get; }
    public DsmNodeVo[] DsmNodes { get; }
    public DsmElement1d[] DsmElement1ds { get; }
}

public class DsmNodeFixture
{
    public NodeFixture Fixture { get; }
}

public class DsmElement1dFixture(Element1dFixture fixture)
{
    public Element1dFixture Fixture { get; } = fixture;

    public double[,]? ExpectedRotationMatrix { get; init; }
    public double[,]? ExpectedTransformationMatrix { get; init; }
    public double[,]? ExpectedLocalStiffnessMatrix { get; init; }
    public double[,]? ExpectedGlobalStiffnessMatrix { get; init; }
    public double[]? ExpectedLocalFixedEndForces { get; init; }
    public double[]? ExpectedGlobalFixedEndForces { get; init; }
    public double[]? ExpectedLocalEndDisplacements { get; init; }
    public double[]? ExpectedGlobalEndDisplacements { get; init; }
    public double[]? ExpectedLocalEndForces { get; init; }
    public double[]? ExpectedGlobalEndForces { get; init; }
}

public interface IHasStructuralStiffnessMatrix : IDsmModelFixture
{
    public double[,] ExpectedStructuralStiffnessMatrix { get; }
}

//public static class IHasStructuralStiffnessMatrixExtensions
//{
//    public static
//}

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
