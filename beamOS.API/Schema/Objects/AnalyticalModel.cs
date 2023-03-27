using LanguageExt;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Objects.Geometry;
using Objects.Structural.Results;
using Speckle.Core.Kits;
using System.Collections;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace beamOS.API.Schema.Objects
{
  public sealed partial class AnalyticalModel : Base<AnalyticalModel>
  {
    public AnalyticalModel() { }
    public AnalyticalModel(double[] initialPoint) 
    { 
      OctreeRoot = new ModelOctreeNode(
        this, 
        new Point(initialPoint[0], initialPoint[1], initialPoint[2]),
        MinTreeNodeSize,
        Option<ModelOctreeNode>.None
      );  
    }

    public AnalyticalModel(ModelOctreeNode root)
    {
      OctreeRoot = root;
    }

    private readonly Dictionary<int, Node> _nodes = new();
    public IReadOnlyDictionary<int, Node> Nodes => _nodes;
    private readonly Dictionary<int, Element1D> _element1Ds = new();
    public IReadOnlyDictionary<int, Element1D> Element1Ds => _element1Ds;
    public double TOLERENCE = 1;
    public float MinTreeNodeSize { get; set; } = 5;
    public int ElementsPerTreeNode { get; set; } = 10;

    //public void AddOrGetNode(Node node)
    public void AddNode(Node node, out Option<Node> existingNodeOption)
    {
      var smallestTreeNode = OctreeRoot.SmallestTreeNodeContainingPoint(node.GetPoint())
        .IfNone(() => {
          ExpandOctree(node.GetPoint());
          return OctreeRoot;
        });

      existingNodeOption = smallestTreeNode.GetExistingNodeAtThisLevel(node.GetPoint());

      // TODO
      //existingNodeOption.IfSome(existingNodeOption => UpdateNode());

      existingNodeOption.IfNone(() =>
      {
        node.Id = _nodes.Count;
        _nodes.Add(_nodes.Count, node);
        smallestTreeNode.AddNode(node);
      });
    }

    public void AddElement1D(Element1D el)
    {
      AddNode(el.BaseCurve.EndNode0, out var existingNodeOption);
      existingNodeOption.IfSome(node => el.BaseCurve.EndNode0 = node);

      AddNode(el.BaseCurve.EndNode1, out existingNodeOption);
      existingNodeOption.IfSome(node => el.BaseCurve.EndNode1 = node);

      // TODO: check if model already has object defined between these two points
      el.Id = _element1Ds.Count;
      _element1Ds.Add(el.Id, el);
    }

    public sealed record DofInfo(int NodeId, int DofIndex);

    [ClearOnModelUnlock]
    public List<DofInfo>? _dofs;
    public List<DofInfo> DOFs
    {
      get
      {
        if (_dofs != null)
          return _dofs;

        _dofs = new List<DofInfo>();
        var knownDisplacements = new List<DofInfo>();
        var knowForces = new List<DofInfo>();
        foreach (var node in _nodes.Values)
        {
          for (var i = 0; i < 6; i++)
          {
            if (node.DOFs[i])
              knowForces.Add(new DofInfo(node.Id, i));
            else
              knownDisplacements.Add(new DofInfo(node.Id, i));
          }
        }
        _dofs.AddRange(knownDisplacements);
        _dofs.AddRange(knowForces);    
        return _dofs;
      }
    }

    public bool UnlockObject(object? obj)
    {
      if (obj == null) return false;
      //if (!Unlockable(obj)) return;
      var objUnlocked = false;
      var type = obj.GetType();

      if (obj is IList list)
      {
        if (list.Count == 0)
          return false;

        // unlock the first item in a list first and check if it was actually unlocked or not
        objUnlocked = UnlockObject(list[0]);

        // if it was not unlocked, assume that the rest of the list does not need to be unlocked
        if (!objUnlocked)
          return false;

        for (var i = 1; i < list.Count; i++)
          UnlockObject(list[i]);
      }
      else if (obj is IDictionary dict)
      {
        if (dict.Count == 0)
          return false;

        var valueEnumerator = dict.Values.GetEnumerator();
        var keyEnumerator = dict.Keys.GetEnumerator();

        // unlock the first item in a list first and check if it was actually unlocked or not
        valueEnumerator.MoveNext();
        var valueObjUnlocked = UnlockObject(valueEnumerator.Current);
        keyEnumerator.MoveNext();
        var keyObjUnlocked = UnlockObject(keyEnumerator.Current);

        // if it was unlocked, assume that the rest of the list needs to be unlocked
        if (valueObjUnlocked)
        {
          objUnlocked = true;
          while (valueEnumerator.MoveNext())
          {
            UnlockObject(valueEnumerator.Current);
          }
        }

        if (keyObjUnlocked)
        {
          objUnlocked = true;
          while (keyEnumerator.MoveNext())
          {
            UnlockObject(keyEnumerator.Current);
          }
        }
      }

      if (type.IsPrimitive || type.IsEnum)
        return objUnlocked;

      if (!(type.Namespace is string space && this.GetType().Namespace is string objectsSpace && space.Contains(objectsSpace)))
        return objUnlocked;

      var props = type.GetProperties();
      foreach (var prop in props)
      {
        var att = Attribute.GetCustomAttribute(prop, typeof(ClearOnModelUnlock));
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
        
        UnlockObject(newValueToCheck);
      }

      var fields = type.GetFields();
      foreach (var field in fields)
      {
        var att = Attribute.GetCustomAttribute(field, typeof(ClearOnModelUnlock));
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
        UnlockObject(newValueToCheck);
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
}
