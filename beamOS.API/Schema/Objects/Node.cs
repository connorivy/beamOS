namespace beamOS.API.Schema.Objects
{
  public class Node
  {
    public int Id { get; set; }
    // [x, y, z]
    public double[] Position { get; set; } = new double[3];
    public double[] Stiffness { get; set; } = new double[6];
    // DOFS Fx Fy Fz Mx My Mz
    // Default fixity is free because this is the most conservative assumption
    public bool[] DOFs { get; set; } = new bool[6] { true, true, true, true, true, true };
    // DOFS Fx Fy Fz Mx My Mz
    public double[] Reactions { get; set; } = new double[6];
  }
}
