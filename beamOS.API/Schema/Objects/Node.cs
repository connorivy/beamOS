namespace beamOS.API.Schema.Objects;

using global::Objects.Geometry;

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
<<<<<<< HEAD
    if (position.Length != 3)
    {
      throw new ArgumentException("Node position must be of the form [x,y,z]");
    }

    if (dofs is bool[] dofsNotNull)
    {
      if (dofsNotNull.Length != 6)
      {
        throw new ArgumentException("Node DOFs must be an array of length 6");
      }

      this.DOFs = dofsNotNull;
    }
    else
    {
      this.DOFs = new bool[6] { true, true, true, true, true, true };
    }

    this.Position = position;
=======
    public int Id { get; set; }
    // [x, y, z]
    public double[] Position { get; set; } = new double[3];
    public double[] Stiffness { get; set; } = new double[6];
    // DOFS Fx Fy Fz Mx My Mz
    // Default fixity is free because this is the most conservative assumption
    public bool[] DOFs { get; set; } = new bool[6] { true, true, true, true, true, true };
    // DOFS Fx Fy Fz Mx My Mz
    public double[] Reactions { get; set; } = new double[6];
    public Node(double[] position) 
    {
      if (position.Length != 3)
        throw new ArgumentException("Node position must be of the form [x,y,z]");
      
      Position = position;
    }
>>>>>>> main
  }
  public Point GetPoint() => new(this.Position[0], this.Position[1], this.Position[2]);
}
