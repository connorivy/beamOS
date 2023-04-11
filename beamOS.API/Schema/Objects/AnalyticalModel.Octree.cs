namespace beamOS.API.Schema.Objects;
using LanguageExt;
using global::Objects.Geometry;
using System.Diagnostics;

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
    private readonly Dictionary<int, Node> _nodes = new();
    public IReadOnlyDictionary<int, Node> Nodes => _nodes;
    private readonly Dictionary<int, Element1D> _element1Ds = new();
    public IReadOnlyDictionary<int, Element1D> Element1Ds => _element1Ds;
    public bool Partitioned = false;

    public Point Min { get; private set; }
    public Point Max { get; private set; }

    public ModelOctreeNode(AnalyticalModel model, Point center, double size, Option<ModelOctreeNode> oldTreeRootOption)
    {
      this.model = model;
      Center = center;
      Size = size;
      Min = new Point(
        Center.x - Size / 2,
        Center.y - Size / 2,
        Center.z - Size / 2
      );
      Max = new Point(
        Center.x + Size / 2,
        Center.y + Size / 2,
        Center.z + Size / 2
      );

      oldTreeRootOption.IfSome(oldTreeRoot =>
      {
        CreateChildTreeNodes();
        SetChildTreeNode(oldTreeRoot);
      });
    }
    private void SetChildTreeNode(ModelOctreeNode oldTreeRoot)
    {
      Debug.WriteLine($"{oldTreeRoot.Center.x}, {oldTreeRoot.Center.y}, {oldTreeRoot.Center.z}, {Size / 3}");
      for (var i = 0; i < ChildTreeNodes.Length; i++)
      {
        Debug.WriteLine($"{ChildTreeNodes[i].Center.x}, {ChildTreeNodes[i].Center.y}, {ChildTreeNodes[i].Center.z}, {oldTreeRoot.Center.DistanceTo(ChildTreeNodes[i].Center)}");
        if (oldTreeRoot.Center.DistanceTo(ChildTreeNodes[i].Center) > Size / 3)
          continue;

        ChildTreeNodes[i] = oldTreeRoot;
        return;
      }

      throw new Exception("Trying to set child tree node that is not within the parent");
    }
    public bool Contains(Point p) => p.x >= Center.x - Size / 2 &&
        p.x < Center.x + Size / 2 &&
        p.y >= Center.y - Size / 2 &&
        p.y < Center.y + Size / 2 &&
        p.z >= Center.z - (Size / 2) &&
        p.z < Center.z + (Size / 2);
    public Option<ModelOctreeNode> SmallestTreeNodeContainingPoint(Point p)
    {
      if (!Contains(p)) return Option<ModelOctreeNode>.None;

      if (!Partitioned) return this;

      foreach (var child in ChildTreeNodes)
      {
        var modelNode = child.SmallestTreeNodeContainingPoint(p);
        if (modelNode.IsSome) return modelNode;
      }

      throw new Exception("I wrote this logic wrong, this line should never be reached");
    }
    public Point GetPointOffsetDirections(Point p)
    {
      return new Point(
        GetSingleOffsetDirection(p.x - Center.x),
        GetSingleOffsetDirection(p.y - Center.y),
        GetSingleOffsetDirection(p.z - Center.z)
      );
    }
    public int GetSingleOffsetDirection(double x)
    {
      return x switch
      {
        < 0 => -1,
        > 0 => 1,
        _ => 0
      };
    }
    public Option<Node> GetExistingNodeAtThisLevel(Point location)
    {
      foreach (var node in Nodes.Values)
      {
        var nodeVect = new Point(node.Position[0], node.Position[1], node.Position[2]);
        var dist = location.DistanceTo(nodeVect);
        if (dist < model.TOLERENCE)
          return node;
      }
      return Option<Node>.None;
    }
    public Option<Node> GetExistingNodeAtAnyLevel(Point location)
    {
      var analyticalNode = GetExistingNodeAtThisLevel(location);
      if (analyticalNode.IsSome)
        return analyticalNode;

      foreach (var node in ChildTreeNodes)
      {
        analyticalNode = node.GetExistingNodeAtThisLevel(location);
        if (analyticalNode.IsSome)
          return analyticalNode;
      }

      return Option<Node>.None;
    }

    internal void AddNode(Node node)
    {
      _nodes.Add(node.Id, node);
      if (Size / 2 >= model.MinTreeNodeSize && _nodes.Count + _element1Ds.Count > model.ElementsPerTreeNode)
      {
        Partition();
      }
    }
    internal void AddElement1D(Element1D el)
    {
      _element1Ds.Add(el.Id, el);
      if (Size / 2 >= model.MinTreeNodeSize && _nodes.Count + _element1Ds.Count > model.ElementsPerTreeNode)
      {
        Partition();
      }
    }
    internal void Partition()
    {
      if (Partitioned)
        throw new Exception("Trying to partition an already partitioned tree node");

      CreateChildTreeNodes();
      AssignElementsToChildTreeNodes();
    }
    internal void CreateChildTreeNodes()
    {
      Partitioned = true;
      for (var i = 0; i < ChildTreeNodes.Length; i++)
      {
        var offsetDirectionZ = i % 2;
        var offsetDirectionY = Math.Floor(i / 2.0) % 2;
        var offsetDirectionX = Math.Floor(i / 4.0) % 2;
        var childCenter = new Point(
          Center.x - Size / 4 * (offsetDirectionX == 0 ? -1 : 1),
          Center.y - Size / 4 * (offsetDirectionY == 0 ? -1 : 1),
          Center.z - Size / 4 * (offsetDirectionZ == 0 ? -1 : 1)
        );

        ChildTreeNodes[i] = new ModelOctreeNode(model, childCenter, Size / 2, Option<ModelOctreeNode>.None);
      }
    }
    internal void AssignElementsToChildTreeNodes()
    {
      foreach (var node in _nodes.Values)
      {
        var childTreeNodeOption = SmallestTreeNodeContainingPoint(node.GetPoint());
        childTreeNodeOption.Match(
          Some: treeNode => {
            treeNode.AddNode(node);
            _nodes.Remove(node.Id);
          },
          None: () => throw new Exception($"Could not find child node that contains node {node}")
        );
      }

      foreach (var el in _element1Ds.Values)
      {
        foreach (var treeNode in ChildTreeNodes)
        {
          if (treeNode.LineIntersects(el.BaseCurve.EndNode0.GetPoint(), el.BaseCurve.EndNode1.GetPoint()))
            treeNode.AddElement1D(el);
        }
      }
    }

    public bool LineIntersects(Point p0, Point p1)
    {
      var direction = p1 - p0;
      double tmin = (Min.x - p0.x) / direction.x;
      double tmax = (Max.x - p0.x) / direction.x;

      if (tmin > tmax)
      {
        double temp = tmin;
        tmin = tmax;
        tmax = temp;
      }

      double tymin = (Min.y - p0.y) / direction.y;
      double tymax = (Max.y - p0.y) / direction.y;

      if (tymin > tymax)
      {
        double temp = tymin;
        tymin = tymax;
        tymax = temp;
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

      double tzmin = (Min.z - p0.z) / direction.z;
      double tzmax = (Max.z - p0.z) / direction.z;

      if (tzmin > tzmax)
      {
        double temp = tzmin;
        tzmin = tzmax;
        tzmax = temp;
      }

      if ((tmin > tzmax) || (tzmin > tmax))
      {
        return false;
      }

      return true;
    }
  }
}
