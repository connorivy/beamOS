namespace beamOS.API.Schema.Objects;
using LanguageExt;
using global::Objects.Geometry;
using System.Collections;
using System.Reflection;
using System.Text.Json.Serialization;

public sealed partial class AnalyticalModel : Base<AnalyticalModel>
{
  public AnalyticalModel() { }
  public AnalyticalModel(params double[] initialPoint) => this.OctreeRoot = new OctreeNode(
      this,
      new Point(initialPoint[0], initialPoint[1], initialPoint[2]),
      this.MinTreeNodeSize,
      Option<OctreeNode>.None
    );

  public AnalyticalModel(Option<float> minTreeNodeSize, params double[] initialPoint)
  {
    _ = minTreeNodeSize.IfSome(size => this.MinTreeNodeSize = size);
    this.OctreeRoot = new OctreeNode(
      this,
      new Point(initialPoint[0], initialPoint[1], initialPoint[2]),
      this.MinTreeNodeSize,
      Option<OctreeNode>.None
    );
  }

  public AnalyticalModel(OctreeNode root) => this.OctreeRoot = root;

  private readonly Dictionary<int, Node> nodes = new();
  public IReadOnlyDictionary<int, Node> Nodes => this.nodes;
  private readonly Dictionary<int, Element1D> element1Ds = new();
  public IReadOnlyDictionary<int, Element1D> Element1Ds => this.element1Ds;
  public double TOLERENCE { get; set; } = 1;
  public float MinTreeNodeSize { get; set; } = 5;
  public int ElementsPerTreeNode { get; set; } = 10;

  //public void AddOrGetNode(Node node)
  public void AddNode(Node node, out Option<Node> existingNodeOption)
  {
    var smallestTreeNode = this.OctreeRoot.SmallestTreeNodeContainingPoint(node.GetPoint())
      .IfNone(() =>
      {
        this.ExpandOctree(node.GetPoint());
        return this.OctreeRoot;
      });

    existingNodeOption = smallestTreeNode.GetExistingNodeAtThisLevel(node.GetPoint());

    // TODO
    //existingNodeOption.IfSome(existingNodeOption => UpdateNode());

    _ = existingNodeOption.IfNone(() =>
    {
      node.Id = this.nodes.Count;
      this.nodes.Add(this.nodes.Count, node);
      smallestTreeNode.AddNode(node);
    });
  }

  public void AddElement1D(Element1D el)
  {
    this.AddNode(el.BaseCurve.EndNode0, out var existingNodeOption);
    _ = existingNodeOption.IfSome(node => el.BaseCurve.EndNode0 = node);

    this.AddNode(el.BaseCurve.EndNode1, out existingNodeOption);
    _ = existingNodeOption.IfSome(node => el.BaseCurve.EndNode1 = node);

    // TODO: check if model already has object defined between these two points
    el.Id = this.element1Ds.Count;
    this.element1Ds.Add(el.Id, el);
  }

  public sealed record DofInfo(int NodeId, int DofIndex);

  [ClearOnModelUnlock]
  public List<DofInfo>? dofs;
  [JsonIgnore]
  public List<DofInfo> DOFs
  {
    get
    {
      if (this.dofs != null)
      {
        return this.dofs;
      }

      this.dofs = new List<DofInfo>();
      var knownDisplacements = new List<DofInfo>();
      var knowForces = new List<DofInfo>();
      foreach (var node in this.nodes.Values)
      {
        for (var i = 0; i < 6; i++)
        {
          if (node.DOFs[i])
          {
            knowForces.Add(new DofInfo(node.Id, i));
          }
          else
          {
            knownDisplacements.Add(new DofInfo(node.Id, i));
          }
        }
      }
      this.dofs.AddRange(knownDisplacements);
      this.dofs.AddRange(knowForces);
      return this.dofs;
    }
  }

  public bool UnlockObject(object? obj)
  {
    if (obj == null)
    {
      return false;
    }

    var objUnlocked = false;
    var type = obj.GetType();

    if (obj is IList list)
    {
      if (list.Count == 0)
      {
        return false;
      }

      // unlock the first item in a list first and check if it was actually unlocked or not
      objUnlocked = this.UnlockObject(list[0]);

      // if it was not unlocked, assume that the rest of the list does not need to be unlocked
      if (!objUnlocked)
      {
        return false;
      }


      for (var i = 1; i < list.Count; i++)
      {
        _ = this.UnlockObject(list[i]);
      }
    }
    else if (obj is IDictionary dict)
    {
      if (dict.Count == 0)
      {
        return false;
      }

      var valueEnumerator = dict.Values.GetEnumerator();
      var keyEnumerator = dict.Keys.GetEnumerator();

      // unlock the first item in a list first and check if it was actually unlocked or not
      _ = valueEnumerator.MoveNext();
      _ = keyEnumerator.MoveNext();

      // if it was unlocked, assume that the rest of the list needs to be unlocked
      if (this.UnlockObject(valueEnumerator.Current))
      {
        objUnlocked = true;
        while (valueEnumerator.MoveNext())
        {
          _ = this.UnlockObject(valueEnumerator.Current);
        }
      }

      if (this.UnlockObject(keyEnumerator.Current))
      {
        objUnlocked = true;
        while (keyEnumerator.MoveNext())
        {
          _ = this.UnlockObject(keyEnumerator.Current);
        }
      }
    }

    if (type.IsPrimitive || type.IsEnum)
    {
      return objUnlocked;
    }

    if (!(type.Namespace is string space && this.GetType().Namespace is string objectsSpace && space.Contains(objectsSpace)))
    {
      return objUnlocked;
    }

    var props = type.GetProperties();
    foreach (var prop in props)
    {
      var att = Attribute.GetCustomAttribute(prop, typeof(ClearOnModelUnlockAttribute));
      if (att != null)
      {
        prop.SetValue(obj, null);
        objUnlocked = true;
      }

      object? newValueToCheck;
      try
      {
        newValueToCheck = prop.GetValue(obj);
      }
      catch (TargetParameterCountException)
      {
        continue;
      }

      _ = this.UnlockObject(newValueToCheck);
    }

    var fields = type.GetFields();
    foreach (var field in fields)
    {
      var att = Attribute.GetCustomAttribute(field, typeof(ClearOnModelUnlockAttribute));
      if (att != null)
      {
        try
        {
          field.SetValue(obj, null);
        }
        catch
        {
          throw new Exception($"Cannot apply attribute {att} to non nullable field {field.Name} on object type {type.Name}");
        }
        objUnlocked = true;
      }

      object? newValueToCheck;
      try
      {
        newValueToCheck = field.GetValue(obj);
      }
      catch (TargetParameterCountException)
      {
        continue;
      }
      _ = this.UnlockObject(newValueToCheck);
    }

    return objUnlocked;
  }

  //private Matrix<double>? _globalStiffnessMatrix = null;
  //public Matrix<double> GlobalStiffnessMatrix
  //{
  //  get
  //  {
  //  }
  //}
}
