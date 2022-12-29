namespace beamOS.API.Schema.Objects
{
  public interface ICurve
  {
    /// <summary>
    /// The length of the curve.
    /// </summary>
    double Length { get; }

    double[] P0 { get; set; }
    double[] P1 { get; set; }
  }
}
