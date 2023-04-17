namespace beamOS.API.Schema.Objects;
public class SectionProfile : Base<SectionProfile>
{
  public SectionProfile() { }
  public SectionProfile(string? name, double iz, double iy, double a, double j)
  {
<<<<<<< HEAD
    this.Name = name;
    this.Iz = iz;
    this.Iy = iy;
    this.A = a;
    this.J = j;
=======
    public string Name { get; set; }
    // moment of inertia about the strong axis
    public double Iz { get; set; }
    // moment of inertia about the weak axis
    public double Iy { get; set; }
    // cross sectional area
    public double A { get; set; }
    // polar moment of inertia or polar second moment of area
    public double J { get; set; }
>>>>>>> main
  }
  public string? Name { get; set; }
  // moment of inertia about the strong axis
  public double Iz { get; set; }
  // moment of inertia about the weak axis
  public double Iy { get; set; }
  // cross sectional area
  public double A { get; set; }
  // polar moment of inertia or polar second moment of area
  public double J { get; set; }
}
