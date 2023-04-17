namespace beamOS.API.Schema.Objects;
public class Material : Base<Material>
{
  public Material() { }
  public Material(string? name, double e, double g)
  {
    this.Name = name;
    this.E = e;
    this.G = g;
  }
  public string? Name { get; set; }
  // modulus of elasticity
  public double E { get; set; }
  // modulus of rigidity
  public double G { get; set; }
}
