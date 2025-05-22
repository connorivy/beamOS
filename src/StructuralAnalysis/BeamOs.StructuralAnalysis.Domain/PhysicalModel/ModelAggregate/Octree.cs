using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

public class Point3D
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public Point3D() { }

    public Point3D(double x, double y, double z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }
}

public class OctreeNode : BeamOsEntity<OctreeNodeId>
{
    private const int MaxObjects = 8;
    private const int MaxDepth = 8;

    public Point3D Center { get; private set; }
    public double Length { get; private set; }
    public List<Node> Objects { get; private set; }
    public OctreeNode[]? Children { get; private set; }
    public int Depth { get; private set; }

    public OctreeNode(Point3D center, double length, int depth = 0)
    {
        this.Center = center;
        this.Length = length;
        this.Depth = depth;
        this.Objects = new List<Node>();
        this.Children = null;
    }

    // For EF Core
    protected OctreeNode()
    {
        this.Center = new Point3D();
        this.Length = 0;
        this.Depth = 0;
        this.Objects = new List<Node>();
        this.Children = null;
    }

    private static Point3D ToPoint3D(Point point)
    {
        return new Point3D(point.X.Meters, point.Y.Meters, point.Z.Meters);
    }

    public void Insert(Node node)
    {
        Point3D position = ToPoint3D(node.LocationPoint);
        this.Insert(node, position);
    }

    private void Insert(Node node, Point3D position)
    {
        if (this.Children != null)
        {
            int index = this.GetChildIndex(position);
            this.Children[index].Insert(node);
            return;
        }

        this.Objects.Add(node);
        if (this.Objects.Count > MaxObjects && this.Depth < MaxDepth)
        {
            this.Subdivide();
            foreach (Node obj in this.Objects.ToList())
            {
                this.Children![this.GetChildIndex(ToPoint3D(obj.LocationPoint))].Insert(obj);
            }
            this.Objects.Clear();
        }
    }

    private void Subdivide()
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
                    Point3D childCenter = new Point3D(
                        this.Center.X + (dx * offset),
                        this.Center.Y + (dy * offset),
                        this.Center.Z + (dz * offset)
                    );
                    this.Children[i++] = new OctreeNode(childCenter, childLength, this.Depth + 1);
                }
            }
        }
    }

    private int GetChildIndex(Point3D pos)
    {
        int index = 0;
        if (pos.X >= this.Center.X)
        {
            index |= 1;
        }
        if (pos.Y >= this.Center.Y)
        {
            index |= 2;
        }
        if (pos.Z >= this.Center.Z)
        {
            index |= 4;
        }
        return index;
    }

    private Point3D Min
    {
        get
        {
            return new Point3D(
                this.Center.X - (this.Length / 2.0),
                this.Center.Y - (this.Length / 2.0),
                this.Center.Z - (this.Length / 2.0)
            );
        }
    }
    private Point3D Max
    {
        get
        {
            return new Point3D(
                this.Center.X + (this.Length / 2.0),
                this.Center.Y + (this.Length / 2.0),
                this.Center.Z + (this.Length / 2.0)
            );
        }
    }

    public void FindNodesWithin(Point3D search, double tolerance, List<Node> result)
    {
        if (!this.IntersectsSphere(search, tolerance))
        {
            return;
        }

        foreach (Node node in this.Objects)
        {
            Point loc = node.LocationPoint;
            double dist = Math.Sqrt(
                Math.Pow(loc.X.Meters - search.X, 2)
                    + Math.Pow(loc.Y.Meters - search.Y, 2)
                    + Math.Pow(loc.Z.Meters - search.Z, 2)
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

    private bool IntersectsSphere(Point3D center, double radius)
    {
        // Clamp center to the AABB and compute squared distance
        Point3D min = this.Min;
        Point3D max = this.Max;
        double dx = Math.Max(min.X - center.X, 0);
        dx = Math.Max(dx, center.X - max.X);
        double dy = Math.Max(min.Y - center.Y, 0);
        dy = Math.Max(dy, center.Y - max.Y);
        double dz = Math.Max(min.Z - center.Z, 0);
        dz = Math.Max(dz, center.Z - max.Z);
        double distSq = (dx * dx) + (dy * dy) + (dz * dz);
        return distSq <= (radius * radius);
    }
}

public class Octree : BeamOsModelEntity<OctreeId>
{
    public OctreeNode Root { get; private set; }

    public Octree(ModelId modelId, Point3D center, double length, OctreeId? id = null)
        : base(id ?? new(), modelId)
    {
        this.Root = new OctreeNode(center, length);
    }

    // For EF Core
    protected Octree()
        : base(default, default)
    {
        this.Root = null!; // EF Core will set this
    }

    public void Add(Node node)
    {
        this.Root.Insert(node);
    }

    public List<Node> FindNodesWithin(Point searchPoint, double toleranceMeters)
    {
        List<Node> result = new List<Node>();
        Point3D search3D = new Point3D(
            searchPoint.X.Meters,
            searchPoint.Y.Meters,
            searchPoint.Z.Meters
        );
        this.Root.FindNodesWithin(search3D, toleranceMeters, result);
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
