using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using FluentAssertions;
using UnitsNet.Units;

namespace BeamOs.Tests.StructuralAnalysis.Unit;

public class OctreeTests
{
    private static readonly Guid ModelId = Guid.NewGuid();

    private static Node CreateNode(double x, double y, double z, int id = 0)
    {
        // Use meters as the default unit
        return new Node(
            modelId: ModelId,
            locationPoint: new Point(x, y, z, LengthUnit.Meter),
            id: id
        );
    }

    [Test]
    public void InsertedNode_CanBeFoundWithinTolerance()
    {
        var octree = new Octree(ModelId, new Point(0, 0, 0, LengthUnit.Meter), 10);
        var node = CreateNode(1, 2, 3);
        octree.Add(node);

        var found = octree.FindNodesWithin(new Point(1, 2, 3, LengthUnit.Meter), 0.01);
        found.Should().Contain(node);
    }

    [Test]
    public void NodeOutsideInitialBounds_ExpandsRootAndIsFound()
    {
        var octree = new Octree(ModelId, new Point(0, 0, 0, LengthUnit.Meter), 2);
        var node = CreateNode(20, 0, 0);
        octree.Add(node);

        var found = octree.FindNodesWithin(new Point(20, 0, 0, LengthUnit.Meter), 0.01);
        found.Should().Contain(node);
    }

    [Test]
    public void FindNodesWithin_ReturnsAllNodesWithinTolerance()
    {
        var octree = new Octree(ModelId, new Point(0, 0, 0, LengthUnit.Meter), 10);
        var node1 = CreateNode(1, 2, 3, id: 1);
        var node2 = CreateNode(1.1, 2, 3, id: 2);
        var node3 = CreateNode(5, 5, 5, id: 3);
        octree.Add(node1);
        octree.Add(node2);
        octree.Add(node3);

        var found = octree.FindNodesWithin(new Point(1, 2, 3, LengthUnit.Meter), 0.05);
        found.Should().Contain(node1);
        found.Should().NotContain(node2);
        found.Should().NotContain(node3);
    }

    [Test]
    public void FindNodesWithin_ReturnsMultipleNodesWithinTolerance()
    {
        var octree = new Octree(ModelId, new Point(0, 0, 0, LengthUnit.Meter), 10);
        var node1 = CreateNode(1, 2, 3, id: 1);
        var node2 = CreateNode(1.1, 2, 3, id: 2);
        var node3 = CreateNode(5, 5, 5, id: 3);
        octree.Add(node1);
        octree.Add(node2);
        octree.Add(node3);

        var found = octree.FindNodesWithin(new Point(1, 2, 3, LengthUnit.Meter), 1);
        found.Should().Contain(node1);
        found.Should().Contain(node2);
        found.Should().NotContain(node3);
    }
}
