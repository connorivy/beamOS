namespace beamOS.API.Schema.Objects;

using global::Objects.Geometry;
using Speckle.Core.Models;

public class Node : Base<Node>
{
  public Node() { }
  public int Id { get; set; }
  /// <summary>
  /// [x, y, z]
  /// </summary>
  public double[] Position { get; set; } = new double[3];
  public double[] Stiffness { get; set; } = new double[6];

  // Default fixity is free because this is the most conservative assumption
  [DetachProperty]
  public DOFs DOFs { get; set; } = DOFs.Free();
  // DOFS Fx Fy Fz Mx My Mz
  public double[] Reactions { get; set; } = new double[6];
  public Node(double[] position, bool[]? dofs = null)
  {
    if (position.Length != 3)
    {
      throw new ArgumentException("Node position must be of the form [x,y,z]");
    }

    if (dofs != null)
    {
      this.DOFs = new DOFs(dofs);
    }

    this.Position = position;
  }
  public Point GetPoint() => new(this.Position[0], this.Position[1], this.Position[2]);
}
