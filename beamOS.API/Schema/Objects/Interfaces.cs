namespace beamOS.API.Schema.Objects
{
  public interface ICurve
  {
    /// <summary>
    /// The length of the curve.
    /// </summary>
    double Length { get; }
    //Node EndNode0 { get; set; }
    //Node EndNode1 { get; set; }
    double[] P0 { get; set; }
    double[] P1 { get; set; }
  }
}
