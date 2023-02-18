using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Objects.Structural.Results;
using System.Collections;
using System.Linq;

namespace beamOS.API.Schema.Objects
{
  public sealed class AnalyticalModel
  {
    public AnalyticalModel() { }

    private readonly List<Node> _nodes = new();
    public IReadOnlyList<Node> Nodes => _nodes.AsReadOnly();
    private readonly List<Element1D> _elementIDs = new();
    public IReadOnlyList<Element1D> Element1Ds => _elementIDs.AsReadOnly();

    public void AddNode(Node node)
    {
      // TODO: make sure there aren't any existing _nodes within a certain tolerance
      node.Id = _nodes.Count;
      _nodes.Add(node);
    }
    public void AddElement1D(Element1D el)
    {
      // TODO: check if model already has object defined between these two points
      AddNode(el.BaseCurve.EndNode0);
      AddNode(el.BaseCurve.EndNode1);
      _elementIDs.Add(el);
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
        foreach (var node in _nodes)
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

    public bool UnlockObject(object obj)
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
        UnlockObject(prop.GetValue(obj));
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
        UnlockObject(field.GetValue(obj));
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
