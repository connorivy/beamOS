namespace beamOS.API.Schema.Objects;

using System.Text;
using beamOS.Tests.Schema.Objects;

/// <summary>
/// Describes the degrees of freedom of an element or node. A true value denotes free movement
/// while a false value denotes restraint
/// </summary>
public class DOFs : CoordinateDirections<bool>
{
  [Obsolete("Used only for deserialization", true)]
  public DOFs() { }
  public DOFs(params bool[] dofs) : base(dofs) { }
  public static DOFs Free() => new(true, true, true, true, true, true);
  public static DOFs Fixed() => new(false, false, false, false, false, false);

  public string GetCode()
  {
    var sb = new StringBuilder();
    foreach (var dof in this.directions)
    {
      _ = sb.Append(dof ? "R" : "F");
    }
    return sb.ToString();
  }

  public bool GetOrderedDOF(int i) => this.directions[i];
}
