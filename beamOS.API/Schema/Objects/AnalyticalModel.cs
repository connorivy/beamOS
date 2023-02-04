using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Objects.Structural.Results;
using System.Collections;
using System.Linq;

namespace beamOS.API.Schema.Objects
{
  public class AnalyticalModel
  {
    public List<Node> Nodes { get; set; } = new List<Node>();
    public List<Element1D> Element1Ds { get; set; } = new List<Element1D>();
    public AnalyticalModel() { }
    public void AddNode(Node node)
    {
      // TODO: make sure there aren't any existing nodes within a certain tolerance
      node.Id = Nodes.Count;
      Nodes.Add(node);
    }
    public void AddElement1D(Element1D el)
    {
      // TODO: check if model already has object defined between these two points
      AddNode(el.BaseCurve.EndNode0);
      AddNode(el.BaseCurve.EndNode1);
      Element1Ds.Add(el);
    }

    public class dofInfo
    {
      public int NodeId { get; set; }
      // dofIndex is the number corrosponding to the DOFs property of the node
      // DOFS -> [Fx, Fy, Fz, Mx, My, Mz]
      public int DofIndex { get; set; }
      public dofInfo(int id, int index)
      {
        NodeId = id;
        DofIndex = index;
      }
    }

    [ClearOnModelUnlock]
    public List<dofInfo>? _dofs;
    public List<dofInfo> DOFs
    {
      get
      {
        if (_dofs != null)
          return _dofs;

        _dofs = new List<dofInfo>();
        var knownDisplacements = new List<dofInfo>();
        var knowForces = new List<dofInfo>();
        foreach (var node in Nodes)
        {
          for (var i = 0; i < 6; i++)
          {
            if (node.DOFs[i])
              knowForces.Add(new dofInfo(node.Id, i));
            else
              knownDisplacements.Add(new dofInfo(node.Id, i));
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
