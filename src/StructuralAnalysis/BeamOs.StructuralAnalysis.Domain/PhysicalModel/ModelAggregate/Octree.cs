using System.Globalization;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

public class OctreeNode : BeamOsEntity<OctreeNodeId>
{
    private const int MaxObjects = 8;
    private const int MaxDepth = 8;

    public Point Center { get; private set; }
    public double Length { get; private set; }
    public List<Node> Objects { get; private set; }
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
    internal void Insert(Node node, Point position)
    {
        if (this.Children != null)
        {
            int index = this.GetChildIndex(position);
            this.Children[index].Insert(node, position);
            return;
        }

        this.Objects.Add(node);
        if (this.Objects.Count > MaxObjects && this.Depth < MaxDepth)
        {
            this.Subdivide();
            foreach (Node obj in this.Objects.ToList())
            {
                Point objPos = obj.LocationPoint;
                this.Children![this.GetChildIndex(objPos)].Insert(obj, objPos);
            }
            this.Objects.Clear();
        }
    }

    public void Subdivide()
    {
        this.Children = new OctreeNode[8];
        double childLength = this.Length / 2.0;
        double offset = childLength / 2.0;
        int i = 0;
        for (int dx = -1; dx <= 1; dx += 2)
        {
            for (int dy = -1; dy <= 1; dy += 2)
            {
                for (int dz = -1; dz <= 1; dz += 2)
                {
                    // Use object initializer for Point
                    Point childCenter = new(
                        this.Center.X.Meters + (dx * offset),
                        this.Center.Y.Meters + (dy * offset),
                        this.Center.Z.Meters + (dz * offset),
                        this.Center.X.Unit
                    );
                    this.Children[i++] = new OctreeNode(childCenter, childLength, this.Depth + 1);
                }
            }
        }
    }

    private int GetChildIndex(Point pos)
    {
        int index = 0;
        if (pos.X.Meters >= this.Center.X.Meters)
        {
            index |= 1;
        }
        if (pos.Y.Meters >= this.Center.Y.Meters)
        {
            index |= 2;
        }
        if (pos.Z.Meters >= this.Center.Z.Meters)
        {
            index |= 4;
        }
        return index;
    }

    private Point Min
    {
        get
        {
            return new Point(
                this.Center.X.Meters - (this.Length / 2.0),
                this.Center.Y.Meters - (this.Length / 2.0),
                this.Center.Z.Meters - (this.Length / 2.0),
                this.Center.X.Unit
            );
        }
    }
    private Point Max
    {
        get
        {
            return new Point(
                this.Center.X.Meters + (this.Length / 2.0),
                this.Center.Y.Meters + (this.Length / 2.0),
                this.Center.Z.Meters + (this.Length / 2.0),
                this.Center.X.Unit
            );
        }
    }

    public void FindNodesWithin(Point search, double tolerance, List<Node> result)
    {
        if (!this.IntersectsSphere(search, tolerance))
        {
            return;
        }

        foreach (Node node in this.Objects)
        {
            Point loc = node.LocationPoint;
            double dist = Math.Sqrt(
                Math.Pow(loc.X.Meters - search.X.Meters, 2)
                    + Math.Pow(loc.Y.Meters - search.Y.Meters, 2)
                    + Math.Pow(loc.Z.Meters - search.Z.Meters, 2)
            );
            if (dist <= tolerance)
            {
                result.Add(node);
            }
        }
        if (this.Children != null)
        {
            foreach (OctreeNode child in this.Children)
            {
                child.FindNodesWithin(search, tolerance, result);
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
}

public class Octree : BeamOsModelEntity<OctreeId>
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

    public void Add(Node node)
    {
        Point position = node.LocationPoint;

        while (!this.IsPointWithinRoot(position))
        {
            this.Root = ExpandRootToFit(this.Root, position);
        }
        this.Root.Insert(node, position);
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
        // Determine in which direction to expand
        double dx = (point.X.Meters < oldCenter.X.Meters) ? -1 : 1;
        double dy = (point.Y.Meters < oldCenter.Y.Meters) ? -1 : 1;
        double dz = (point.Z.Meters < oldCenter.Z.Meters) ? -1 : 1;
        double offset = oldLength / 2.0;
        Point newCenter = new(
            oldCenter.X.Meters + (dx * offset),
            oldCenter.Y.Meters + (dy * offset),
            oldCenter.Z.Meters + (dz * offset),
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

    public List<Node> FindNodesWithin(Point searchPoint, double toleranceMeters)
    {
        List<Node> result = [];
        this.Root.FindNodesWithin(searchPoint, toleranceMeters, result);
        return result;
    }
}

public readonly record struct OctreeId : IIntBasedId
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

public readonly record struct OctreeNodeId : IIntBasedId
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
