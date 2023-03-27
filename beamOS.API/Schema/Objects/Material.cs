﻿using Speckle.Core.Models;

namespace beamOS.API.Schema.Objects
{
  public class Material : Base<Material>
  {
    public string Name { get; set; }
    // modulus of elasticity
    public double E { get; set; }
    // modulus of rigidity
    public double G { get; set; }
  }
}
