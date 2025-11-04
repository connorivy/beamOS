using System.Diagnostics;
using System.Globalization;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

internal class OctreeNode : BeamOsEntity<OctreeNodeId>
{
    private const int MaxObjects = 8;
    private const int MaxDepth = 8;

    public Point Center { get; private set; }
    public double Length { get; private set; }
    public List<NodeIdAndLocation> Objects { get; private set; }
    public OctreeNode[]? Children { get; private set; }
    public int Depth { get; private set; }

    public OctreeNode(Point center, double length, int depth = 0)
    {
        this.Center = center;
        this.Length = length;
        this.Depth = depth;
        this.Objects = [];
        this.Children = null;
    }

    // For EF Core
    protected OctreeNode()
    {
        this.Center = new(0, 0, 0, UnitsNet.Units.LengthUnit.Meter);
        this.Length = 0;
        this.Depth = 0;
        this.Objects = [];
        this.Children = null;
    }

    // Remove ToPoint3D, use Point directly
    internal OctreeNodeId Insert(NodeId nodeId, Point position)
    {
        if (this.Children != null)
        {
            int index = this.GetChildIndex(position);
            return this.Children[index].Insert(nodeId, position);
        }

        this.Objects.Add(new(nodeId, position));
        if (this.Objects.Count > MaxObjects && this.Depth < MaxDepth)
        {
            this.Subdivide();
            foreach (var obj in this.Objects.ToList())
            {
                this.Children![this.GetChildIndex(obj.LocationPoint)]
                    .Insert(obj.NodeId, obj.LocationPoint);
            }
            this.Objects.Clear();
        }
        return this.Id;
    }

    public void Subdivide()
    {
        this.Children = new OctreeNode[8];
        double childLength = this.Length / 2.0;
        double offset = childLength / 2.0;
        for (int dx = -1; dx <= 1; dx += 2)
        {
            for (int dy = -1; dy <= 1; dy += 2)
            {
                for (int dz = -1; dz <= 1; dz += 2)
                {
                    Point childCenter = new(
                        this.Center.X.Meters + (dx * offset),
                        this.Center.Y.Meters + (dy * offset),
                        this.Center.Z.Meters + (dz * offset),
                        this.Center.X.Unit
                    );
                    int childIndex = 0;
                    if (dx > 0)
                    {
                        childIndex |= 1;
                    }
                    if (dy > 0)
                    {
                        childIndex |= 2;
                    }
                    if (dz > 0)
                    {
                        childIndex |= 4;
                    }
                    this.Children[childIndex] = new OctreeNode(
                        childCenter,
                        childLength,
                        this.Depth + 1
                    );
                }
            }
        }
    }

    private int GetChildIndex(Point pos)
    {
        int index = 0;
        if (pos.X.Meters > this.Center.X.Meters)
        {
            index |= 1;
        }
        if (pos.Y.Meters > this.Center.Y.Meters)
        {
            index |= 2;
        }
        if (pos.Z.Meters > this.Center.Z.Meters)
        {
            index |= 4;
        }
        return index;
    }

    private Point Min =>
        new Point(
            this.Center.X.Meters - (this.Length / 2.0),
            this.Center.Y.Meters - (this.Length / 2.0),
            this.Center.Z.Meters - (this.Length / 2.0),
            this.Center.X.Unit
        );
    private Point Max =>
        new Point(
            this.Center.X.Meters + (this.Length / 2.0),
            this.Center.Y.Meters + (this.Length / 2.0),
            this.Center.Z.Meters + (this.Length / 2.0),
            this.Center.X.Unit
        );

    public void FindNodeIdsWithin(
        Point search,
        double tolerance,
        List<NodeId> result,
        params Span<NodeId> nodeIdsToIgnore
    )
    {
        if (!this.IntersectsSphere(search, tolerance))
        {
            return;
        }

        foreach (var node in this.Objects)
        {
            if (nodeIdsToIgnore.Contains(node.NodeId))
            {
                continue;
            }

            Point loc = node.LocationPoint;
            double dist = Math.Sqrt(
                Math.Pow(loc.X.Meters - search.X.Meters, 2)
                    + Math.Pow(loc.Y.Meters - search.Y.Meters, 2)
                    + Math.Pow(loc.Z.Meters - search.Z.Meters, 2)
            );
            if (dist <= tolerance)
            {
                result.Add(node.NodeId);
            }
        }
        if (this.Children != null)
        {
            foreach (OctreeNode child in this.Children)
            {
                child.FindNodeIdsWithin(search, tolerance, result, nodeIdsToIgnore);
            }
        }
    }

    private bool IntersectsSphere(Point center, double radius)
    {
        Point min = this.Min;
        Point max = this.Max;
        double dx = Math.Max(min.X.Meters - center.X.Meters, 0);
        dx = Math.Max(dx, center.X.Meters - max.X.Meters);
        double dy = Math.Max(min.Y.Meters - center.Y.Meters, 0);
        dy = Math.Max(dy, center.Y.Meters - max.Y.Meters);
        double dz = Math.Max(min.Z.Meters - center.Z.Meters, 0);
        dz = Math.Max(dz, center.Z.Meters - max.Z.Meters);
        double distSq = (dx * dx) + (dy * dy) + (dz * dz);
        return distSq <= (radius * radius);
    }

    /// <summary>
    /// Removes a node with the given NodeId from the octree.
    /// </summary>
    /// <param name="nodeId"></param>
    /// <param name="location">This is a marker that helps the octree locate the given node.
    /// It does not have to be the exact location of the node, but it should be within the bounds of the same octree node.
    /// </param>
    /// <returns></returns>
    public bool Remove(NodeId nodeId, Point? location = null)
    {
        // If this is a leaf node
        if (this.Children == null)
        {
            int removed = this.Objects.RemoveAll(n => n.NodeId == nodeId);
            Debug.Assert(
                removed <= 1,
                "Octree node should not contain more than one node with the same NodeId."
            );
            return removed > 0;
        }
        // If not a leaf, search children
        bool removedAny = false;
        if (location != null)
        {
            int childIdx = this.GetChildIndex(location);
            removedAny = this.Children[childIdx].Remove(nodeId, location);
        }
        else
        {
            foreach (var child in this.Children)
            {
                removedAny |= child.Remove(nodeId, null);
            }
        }
        return removedAny;
    }
}

internal class Octree : BeamOsModelEntity<OctreeId>
{
    private const double DefaultStartNodeSize = 1.0;
    public OctreeNode Root { get; private set; }

    public Octree(ModelId modelId, Point point, double startNodeSize, OctreeId? id = null)
        : base(id ?? new(), modelId)
    {
        this.Root = new OctreeNode(point, startNodeSize);
    }

    // For EF Core
    protected Octree()
        : base(default, default)
    {
        this.Root = null!; // EF Core will set this
    }

    private bool RootContains(Point pos)
    {
        double half = this.Root.Length / 2.0;
        return pos.X.Meters >= this.Root.Center.X.Meters - half
            && pos.X.Meters <= this.Root.Center.X.Meters + half
            && pos.Y.Meters >= this.Root.Center.Y.Meters - half
            && pos.Y.Meters <= this.Root.Center.Y.Meters + half
            && pos.Z.Meters >= this.Root.Center.Z.Meters - half
            && pos.Z.Meters <= this.Root.Center.Z.Meters + half;
    }

    public OctreeNodeId Add(Node node)
    {
        Point position = node.LocationPoint;

        while (!this.IsPointWithinRoot(position))
        {
            this.Root = ExpandRootToFit(this.Root, position);
        }
        return this.Root.Insert(node.Id, position);
    }

    public OctreeNodeId Add(
        InternalNode node,
        IReadOnlyDictionary<Element1dId, Element1d> element1dStore,
        IReadOnlyDictionary<NodeId, NodeDefinition> nodeStore
    )
    {
        Point position = node.GetLocationPoint(element1dStore, nodeStore);

        while (!this.IsPointWithinRoot(position))
        {
            this.Root = ExpandRootToFit(this.Root, position);
        }
        return this.Root.Insert(node.Id, position);
    }

    public void Remove(NodeId nodeId, Point? location)
    {
        this.Root.Remove(nodeId, location);
    }

    private bool IsPointWithinRoot(Point point)
    {
        double half = this.Root.Length / 2.0;
        return point.X.Meters >= this.Root.Center.X.Meters - half
            && point.X.Meters <= this.Root.Center.X.Meters + half
            && point.Y.Meters >= this.Root.Center.Y.Meters - half
            && point.Y.Meters <= this.Root.Center.Y.Meters + half
            && point.Z.Meters >= this.Root.Center.Z.Meters - half
            && point.Z.Meters <= this.Root.Center.Z.Meters + half;
    }

    private static OctreeNode ExpandRootToFit(OctreeNode originalRoot, Point point)
    {
        double oldLength = originalRoot.Length;
        double newLength = oldLength * 2.0;
        Point oldCenter = originalRoot.Center;
        // For each axis, offset by +oldLength/2 if point >= oldCenter, else -oldLength/2
        double offsetX =
            (point.X.Meters >= oldCenter.X.Meters) ? oldLength / 2.0 : -oldLength / 2.0;
        double offsetY =
            (point.Y.Meters >= oldCenter.Y.Meters) ? oldLength / 2.0 : -oldLength / 2.0;
        double offsetZ =
            (point.Z.Meters >= oldCenter.Z.Meters) ? oldLength / 2.0 : -oldLength / 2.0;

        Point newCenter = new(
            oldCenter.X.Meters + offsetX,
            oldCenter.Y.Meters + offsetY,
            oldCenter.Z.Meters + offsetZ,
            oldCenter.X.Unit
        );
        OctreeNode newRoot = new(newCenter, newLength);
        newRoot.Subdivide();
        int childIndex = 0;
        if (oldCenter.X.Meters >= newCenter.X.Meters)
        {
            childIndex |= 1;
        }
        if (oldCenter.Y.Meters >= newCenter.Y.Meters)
        {
            childIndex |= 2;
        }
        if (oldCenter.Z.Meters >= newCenter.Z.Meters)
        {
            childIndex |= 4;
        }
        newRoot.Children![childIndex] = originalRoot;

        return newRoot;
    }

    public List<NodeId> FindNodeIdsWithin(
        Point searchPoint,
        double toleranceMeters,
        params Span<NodeId> nodeIdsToIgnore
    )
    {
        List<NodeId> result = [];
        this.Root.FindNodeIdsWithin(searchPoint, toleranceMeters, result, nodeIdsToIgnore);
        return result;
    }
}

internal readonly record struct OctreeId : IIntBasedId
{
    public int Id { get; init; }

    public OctreeId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(OctreeId id) => id.Id;

    public static implicit operator OctreeId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}

internal readonly record struct OctreeNodeId : IIntBasedId
{
    public int Id { get; init; }

    public OctreeNodeId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(OctreeNodeId id) => id.Id;

    public static implicit operator OctreeNodeId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
