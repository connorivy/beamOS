using Objects.Geometry;
using Speckle.Core.Models;

namespace beamOS.API.Schema.Objects
{
  public class Node : Base<Node>
  {
    public Node() { }
    public int Id { get; set; }
    // [x, y, z]
    public double[] Position { get; set; } = new double[3];
    public double[] Stiffness { get; set; } = new double[6];
    // DOFS Fx Fy Fz Mx My Mz
    // Default fixity is free because this is the most conservative assumption
    public bool[] DOFs { get; set; } = new bool[6] { true, true, true, true, true, true };
    // DOFS Fx Fy Fz Mx My Mz
    public double[] Reactions { get; set; } = new double[6];
    public Node(double[] position, bool[]? dofs = null)
    {
      if (position.Length != 3)
        throw new ArgumentException("Node position must be of the form [x,y,z]");

      if (dofs is bool[] dofsNotNull)
      {
        if (dofsNotNull.Length != 6)
          throw new ArgumentException("Node DOFs must be an array of length 6");
        DOFs = dofsNotNull;
      }
      else
        DOFs = new bool[6] { true, true, true, true, true, true };

      Position = position;
    }
    public Point GetPoint() => new(Position[0], Position[1], Position[2]);
  }
}
