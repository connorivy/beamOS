namespace beamOS.API.Schema.Objects;
using System.Diagnostics.Contracts;
using System.Diagnostics;
using global::Objects.Geometry;
using beamOS.API.Schema.Objects.Interfaces;

public sealed class OctreeNode : Base<OctreeNode>
{
  [Obsolete("Serialization only", true)]
  public OctreeNode() { }
  public Point Center { get; private set; }
  public double Size { get; private set; }
  public OctreeNode[] ChildTreeNodes { get; private set; } = new OctreeNode[8];
  private readonly Dictionary<int, Node> nodes = new();
  public IReadOnlyDictionary<int, Node> Nodes => this.nodes;
  private readonly Dictionary<int, Element1D> element1Ds = new();
  public IReadOnlyDictionary<int, Element1D> Element1Ds => this.element1Ds;
  public bool Partitioned { get; set; }

  public Point Min { get; private set; }
  public Point Max { get; private set; }

  private readonly IModelSettings modelSettings;
  public OctreeNode(IModelSettings modelSettings, Point center, double size, OctreeNode? oldTreeRoot)
  {
    this.modelSettings = modelSettings;
    this.Center = center;
    this.Size = size;
    this.Min = new Point(
      this.Center.x - (this.Size / 2),
      this.Center.y - (this.Size / 2),
      this.Center.z - (this.Size / 2)
    );
    this.Max = new Point(
      this.Center.x + (this.Size / 2),
      this.Center.y + (this.Size / 2),
      this.Center.z + (this.Size / 2)
    );

    if (oldTreeRoot != null)
    {
      this.CreateChildTreeNodes();
      this.SetChildTreeNode(oldTreeRoot);
    }
  }

  public OctreeNode(IModelSettings modelSettings, double[] center, double size, OctreeNode? oldTreeRoot)
  {
    this.modelSettings = modelSettings;
    this.Center = new Point(center[0], center[1], center[2]);
    this.Size = size;
    this.Min = new Point(
      this.Center.x - (this.Size / 2),
      this.Center.y - (this.Size / 2),
      this.Center.z - (this.Size / 2)
    );
    this.Max = new Point(
      this.Center.x + (this.Size / 2),
      this.Center.y + (this.Size / 2),
      this.Center.z + (this.Size / 2)
    );
    if (oldTreeRoot != null)
    {
      this.CreateChildTreeNodes();
      this.SetChildTreeNode(oldTreeRoot);
    }
  }
  private void SetChildTreeNode(OctreeNode oldTreeRoot)
  {
    Debug.WriteLine($"{oldTreeRoot.Center.x}, {oldTreeRoot.Center.y}, {oldTreeRoot.Center.z}, {this.Size / 3}");
    for (var i = 0; i < this.ChildTreeNodes.Length; i++)
    {
      Debug.WriteLine($"{this.ChildTreeNodes[i].Center.x}, {this.ChildTreeNodes[i].Center.y}, {this.ChildTreeNodes[i].Center.z}, {oldTreeRoot.Center.DistanceTo(this.ChildTreeNodes[i].Center)}");
      if (oldTreeRoot.Center.DistanceTo(this.ChildTreeNodes[i].Center) > this.Size / 3)
      {
        continue;
      }

      this.ChildTreeNodes[i] = oldTreeRoot;
      return;
    }

    throw new Exception("Trying to set child tree node that is not within the parent");
  }
  [Pure]
  public bool Contains(Point p) => p.x > this.Center.x - (this.Size / 2) &&
      p.x <= this.Center.x + (this.Size / 2) &&
      p.y > this.Center.y - (this.Size / 2) &&
      p.y <= this.Center.y + (this.Size / 2) &&
      p.z > this.Center.z - (this.Size / 2) &&
      p.z <= this.Center.z + (this.Size / 2);
  [Pure]
  public OctreeNode? SmallestTreeNodeContainingPoint(Point p)
  {
    if (!this.Contains(p))
    {
      return null;
    }

    if (!this.Partitioned)
    {
      return this;
    }

    foreach (var child in this.ChildTreeNodes)
    {
      var modelNode = child.SmallestTreeNodeContainingPoint(p);
      if (modelNode != null)
      {
        return modelNode;
      }
    }

    throw new Exception("I wrote this logic wrong, this line should never be reached");
  }
  [Pure]
  public Point GetPointOffsetDirections(Point p) => new(
      GetSingleOffsetDirection(p.x - this.Center.x),
      GetSingleOffsetDirection(p.y - this.Center.y),
      GetSingleOffsetDirection(p.z - this.Center.z)
    );
  [Pure]
  public static int GetSingleOffsetDirection(double x) => x switch
  {
    < 0 => -1,
    > 0 => 1,
    _ => 0
  };
  [Pure]
  public Node? GetExistingNodeAtThisLevel(Point location)
  {
    foreach (var node in this.Nodes.Values)
    {
      var nodeVect = new Point(node.Position[0], node.Position[1], node.Position[2]);
      var dist = location.DistanceTo(nodeVect);
      if (dist < this.modelSettings.Tolerance)
      {
        return node;
      }
    }
    return null;
  }
  [Pure]
  public Node? GetExistingNodeAtAnyLevel(Point location)
  {
    var analyticalNode = this.GetExistingNodeAtThisLevel(location);
    if (analyticalNode != null)
    {
      return analyticalNode;
    }

    foreach (var node in this.ChildTreeNodes)
    {
      analyticalNode = node.GetExistingNodeAtThisLevel(location);
      if (analyticalNode != null)
      {
        return analyticalNode;
      }
    }

    return null;
  }

  internal void AddNode(Node node)
  {
    this.nodes.Add(node.Id, node);
    if (this.Size / 2 >= this.modelSettings.MinTreeNodeSize && this.nodes.Count + this.element1Ds.Count > this.modelSettings.ElementsPerTreeNode)
    {
      this.Partition();
    }
  }
  internal void AddElement1D(Element1D el)
  {
    this.element1Ds.Add(el.Id, el);
    if (this.Size / 2 >= this.modelSettings.MinTreeNodeSize && this.nodes.Count + this.element1Ds.Count > this.modelSettings.ElementsPerTreeNode)
    {
      this.Partition();
    }
  }
  internal void Partition()
  {
    if (this.Partitioned)
    {
      throw new Exception("Trying to partition an already partitioned tree node");
    }

    this.CreateChildTreeNodes();
    this.AssignElementsToChildTreeNodes();
  }
  internal void CreateChildTreeNodes()
  {
    this.Partitioned = true;
    for (var i = 0; i < this.ChildTreeNodes.Length; i++)
    {
      var offsetDirectionZ = i % 2;
      var offsetDirectionY = Math.Floor(i / 2.0) % 2;
      var offsetDirectionX = Math.Floor(i / 4.0) % 2;
      var childCenter = new Point(
        this.Center.x - (this.Size / 4 * (offsetDirectionX == 0 ? -1 : 1)),
        this.Center.y - (this.Size / 4 * (offsetDirectionY == 0 ? -1 : 1)),
        this.Center.z - (this.Size / 4 * (offsetDirectionZ == 0 ? -1 : 1))
      );

      this.ChildTreeNodes[i] = new OctreeNode(this.modelSettings, childCenter, this.Size / 2, null);
    }
  }
  internal void AssignElementsToChildTreeNodes()
  {
    foreach (var node in this.nodes.Values)
    {
      var childTreeNodeOption = this.SmallestTreeNodeContainingPoint(node.GetPoint());
      if (childTreeNodeOption != null)
      {
        childTreeNodeOption.AddNode(node);
        _ = this.nodes.Remove(node.Id);
      }
      else
      {
        throw new Exception($"Could not find child node that contains node {node}");
      }
    }

    foreach (var el in this.element1Ds.Values)
    {
      foreach (var treeNode in this.ChildTreeNodes)
      {
        if (treeNode.CurveIntersects(el.BaseCurve))
        {
          treeNode.AddElement1D(el);
        }
      }
    }
  }

  public bool CurveIntersects(ICurve curve)
  {
    if (curve is not Line line)
    {
      throw new ArgumentException("Non-lines are not yet supported");
    }

    var p0 = line.EndNode0.GetPoint();
    var p1 = line.EndNode1.GetPoint();
    var direction = p1 - p0;
    var tmin = (this.Min.x - p0.x) / direction.x;
    var tmax = (this.Max.x - p0.x) / direction.x;

    if (tmin > tmax)
    {
      (tmax, tmin) = (tmin, tmax);
    }

    var tymin = (this.Min.y - p0.y) / direction.y;
    var tymax = (this.Max.y - p0.y) / direction.y;

    if (tymin > tymax)
    {
      (tymax, tymin) = (tymin, tymax);
    }

    if ((tmin > tymax) || (tymin > tmax))
    {
      return false;
    }

    if (tymin > tmin)
    {
      tmin = tymin;
    }

    if (tymax < tmax)
    {
      tmax = tymax;
    }

    var tzmin = (this.Min.z - p0.z) / direction.z;
    var tzmax = (this.Max.z - p0.z) / direction.z;

    if (tzmin > tzmax)
    {
      (tzmax, tzmin) = (tzmin, tzmax);
    }

    if ((tmin > tzmax) || (tzmin > tmax))
    {
      return false;
    }

    return true;
  }
}
