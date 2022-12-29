namespace beamOS.API.Schema.Objects
{
  public class SectionProfile
  {
    public string Name { get; set; }
    // Moment of Inertia about the strong axis
    public double Ixx { get; set; }
    // Moment of Inertia about the weak axis
    public double Iyy { get; set; }
    // Cross sectional area
    public double A { get; set; }
  }
}
