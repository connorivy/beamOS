namespace beamOS.API.Schema.Objects;
public class Line : Base<Line>, ICurve
{
  public Line() { }
  public Node EndNode0 { get; set; }
  public Node EndNode1 { get; set; }
  public Line(double[] p0, double[] p1)
  {
    this.EndNode0 = new Node(p0);
    this.EndNode1 = new Node(p1);
  }
  public Line((double, double, double) p0, (double, double, double) p1)
    : this(
      new double[] { p0.Item1, p0.Item1, p0.Item3 },
      new double[] { p1.Item1, p1.Item1, p1.Item3 }
    )
  {
  }

  // TODO: maybe implement a private set for length and store that value instead of computing it every time
  public double Length => Math.Sqrt(Math.Pow(this.EndNode1.Position[0] - this.EndNode0.Position[0], 2)
    + Math.Pow(this.EndNode1.Position[1] - this.EndNode0.Position[1], 2)
    + Math.Pow(this.EndNode1.Position[2] - this.EndNode0.Position[2], 2));
}
