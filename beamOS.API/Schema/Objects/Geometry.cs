namespace beamOS.API.Schema.Objects
{
  public class Line : ICurve
  {
    // TODO: maybe implement a private set for length and store that value instead of computing it every time
    public double Length => Math.Sqrt(Math.Pow(P1[0] - P0[0], 2) + Math.Pow(P1[1] - P0[1], 2) + Math.Pow(P1[2] - P0[2], 2));
    public double[] P0 { get; set; } = new double[3];
    public double[] P1 { get; set; } = new double[3];
    public Line(double[] P0, double[] P1) 
    {
      this.P0 = P0;
      this.P1 = P1;
    }
  }
}
