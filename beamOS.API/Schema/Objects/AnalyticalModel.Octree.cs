namespace beamOS.API.Schema.Objects;
using System.Diagnostics;
using global::Objects.Geometry;
using LanguageExt;

public sealed partial class AnalyticalModel
{
  public ModelOctreeNode OctreeRoot { get; private set; }
  public void ExpandOctree(Point p)
  {
    do
    {
      var offsetDirCenterToPoint = this.OctreeRoot.GetPointOffsetDirections(p);
      var newCenter = new Point(
        this.OctreeRoot.Center.x + (this.OctreeRoot.Size / 2 * (offsetDirCenterToPoint.x == 0 ? 1 : offsetDirCenterToPoint.x)),
        this.OctreeRoot.Center.y + (this.OctreeRoot.Size / 2 * (offsetDirCenterToPoint.y == 0 ? 1 : offsetDirCenterToPoint.y)),
        this.OctreeRoot.Center.z + (this.OctreeRoot.Size / 2 * (offsetDirCenterToPoint.z == 0 ? 1 : offsetDirCenterToPoint.z))
      );
      var newRoot = new ModelOctreeNode(this, newCenter, this.OctreeRoot.Size * 2, this.OctreeRoot);
      this.OctreeRoot = newRoot;
    } while (!this.OctreeRoot.Contains(p));
  }

  public sealed class ModelOctreeNode : Base<ModelOctreeNode>
  {
    public ModelOctreeNode() { }
    private readonly AnalyticalModel model;
    public Point Center { get; private set; }
    public double Size { get; private set; }
    public ModelOctreeNode[] ChildTreeNodes { get; private set; } = new ModelOctreeNode[8];
    private readonly Dictionary<int, Node> nodes = new();
    public IReadOnlyDictionary<int, Node> Nodes => this.nodes;
    private readonly Dictionary<int, Element1D> element1Ds = new();
    public IReadOnlyDictionary<int, Element1D> Element1Ds => this.element1Ds;
    public bool Partitioned { get; set; }

    public Point Min { get; private set; }
    public Point Max { get; private set; }

    public ModelOctreeNode(AnalyticalModel model, Point center, double size, Option<ModelOctreeNode> oldTreeRootOption)
    {
      this.model = model;
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

      _ = oldTreeRootOption.IfSome(oldTreeRoot =>
      {
        this.CreateChildTreeNodes();
        this.SetChildTreeNode(oldTreeRoot);
      });
    }
    private void SetChildTreeNode(ModelOctreeNode oldTreeRoot)
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
    public bool Contains(Point p) => p.x >= this.Center.x - (this.Size / 2) &&
        p.x < this.Center.x + (this.Size / 2) &&
        p.y >= this.Center.y - (this.Size / 2) &&
        p.y < this.Center.y + (this.Size / 2) &&
        p.z >= this.Center.z - (this.Size / 2) &&
        p.z < this.Center.z + (this.Size / 2);
    public Option<ModelOctreeNode> SmallestTreeNodeContainingPoint(Point p)
    {
      if (!this.Contains(p))
      {
        return Option<ModelOctreeNode>.None;
      }

      if (!this.Partitioned)
      {
        return this;
      }

      foreach (var child in this.ChildTreeNodes)
      {
        var modelNode = child.SmallestTreeNodeContainingPoint(p);
        if (modelNode.IsSome)
        {
          return modelNode;
        }
      }

      throw new Exception("I wrote this logic wrong, this line should never be reached");
    }
    public Point GetPointOffsetDirections(Point p) => new(
        GetSingleOffsetDirection(p.x - this.Center.x),
        GetSingleOffsetDirection(p.y - this.Center.y),
        GetSingleOffsetDirection(p.z - this.Center.z)
      );
    public static int GetSingleOffsetDirection(double x) => x switch
    {
      < 0 => -1,
      > 0 => 1,
      _ => 0
    };
    public Option<Node> GetExistingNodeAtThisLevel(Point location)
    {
      foreach (var node in this.Nodes.Values)
      {
        var nodeVect = new Point(node.Position[0], node.Position[1], node.Position[2]);
        var dist = location.DistanceTo(nodeVect);
        if (dist < this.model.TOLERENCE)
        {
          return node;
        }
      }
      return Option<Node>.None;
    }
    public Option<Node> GetExistingNodeAtAnyLevel(Point location)
    {
      var analyticalNode = this.GetExistingNodeAtThisLevel(location);
      if (analyticalNode.IsSome)
      {
        return analyticalNode;
      }

      foreach (var node in this.ChildTreeNodes)
      {
        analyticalNode = node.GetExistingNodeAtThisLevel(location);
        if (analyticalNode.IsSome)
        {
          return analyticalNode;
        }
      }

      return Option<Node>.None;
    }

    internal void AddNode(Node node)
    {
      this.nodes.Add(node.Id, node);
      if (this.Size / 2 >= this.model.MinTreeNodeSize && this.nodes.Count + this.element1Ds.Count > this.model.ElementsPerTreeNode)
      {
        this.Partition();
      }
    }
    internal void AddElement1D(Element1D el)
    {
      this.element1Ds.Add(el.Id, el);
      if (this.Size / 2 >= this.model.MinTreeNodeSize && this.nodes.Count + this.element1Ds.Count > this.model.ElementsPerTreeNode)
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

        this.ChildTreeNodes[i] = new ModelOctreeNode(this.model, childCenter, this.Size / 2, Option<ModelOctreeNode>.None);
      }
    }
    internal void AssignElementsToChildTreeNodes()
    {
      foreach (var node in this.nodes.Values)
      {
        var childTreeNodeOption = this.SmallestTreeNodeContainingPoint(node.GetPoint());
        _ = childTreeNodeOption.Match(
          Some: treeNode =>
          {
            treeNode.AddNode(node);
            _ = this.nodes.Remove(node.Id);
          },
          None: () => throw new Exception($"Could not find child node that contains node {node}")
        );
      }

      foreach (var el in this.element1Ds.Values)
      {
        foreach (var treeNode in this.ChildTreeNodes)
        {
          if (treeNode.LineIntersects(el.BaseCurve.EndNode0.GetPoint(), el.BaseCurve.EndNode1.GetPoint()))
          {
            treeNode.AddElement1D(el);
          }
        }
      }
    }

    public bool LineIntersects(Point p0, Point p1)
    {
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
}
