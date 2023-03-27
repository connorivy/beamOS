using Speckle.Core.Models;

namespace beamOS.API.Schema.Objects
{
  public class SectionProfile : Base
  {
    public string Name { get; set; }
    // moment of inertia about the strong axis
    public double Iz { get; set; }
    // moment of inertia about the weak axis
    public double Iy { get; set; }
    // cross sectional area
    public double A { get; set; }
    // polar moment of inertia or polar second moment of area
    public double J { get; set; }
  }
}
