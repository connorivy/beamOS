using beamOS.API.Schema.Objects;
using LanguageExt;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beamOS.Tests.TestObjects.Element1Ds
{
  internal class Element1DFixture
  {
    public Element1DFixture() { }
    public Element1D Element { get; set; }
    public Option<Matrix<double>> ExpectedRotationMatrix { get; set; }
    public Option<Matrix<double>> ExpectedTransformationMatrix { get; set; }
    public Option<Matrix<double>> ExpectedLocalStiffnessMatrix { get; set; }
    public Option<Matrix<double>> ExpectedGlobalStiffnessMatrix { get; set; }
    public Option<Vector<double>> ExpectedLocalFixedEndForces { get; set; }
    public Option<Vector<double>> ExpectedGlobalFixedEndForces { get; set; }
    public Option<Vector<double>> ExpectedLocalEndDisplacements { get; set; }
    public Option<Vector<double>> ExpectedGlobalEndDisplacements { get; set; }
    public Option<Vector<double>> ExpectedLocalEndForces { get; set; }
    public Option<Vector<double>> ExpectedGlobalEndForces { get; set; }
  }
}
