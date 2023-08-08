namespace beamOS.Tests.TestObjects.Element1Ds;
using beamOS.API.Schema.Objects;
using beamOS.Tests.TestObjects.SolvedProblems;
using MathNet.Numerics.LinearAlgebra;

public class AllElement1DFixtures : TheoryData<Element1DFixture>
{
  public List<Element1DFixture> Element1DFixtures { get; } = new()
  {
    // add standalone element 1D fixtures here
  };
  public AllElement1DFixtures()
  {
    var allSolvedProblems = new AllSolvedProblems();
    foreach (var solvedProblem in allSolvedProblems.AllTestObjects)
    {
      foreach (var element1DFixture in solvedProblem.Element1DFixtures)
      {
        this.Add(element1DFixture);
      }
    }

    foreach (var element1DFixture in this.Element1DFixtures)
    {
      this.Add(element1DFixture);
    }
  }
}

public class Element1DFixture : SerializableFixtureBase, IHasLocalResults, IHasGlobalResults
{
  public Element1DFixture()
  {
  }
  public Element1DFixture(Element1D element) => this.Element = element;
  public Element1D Element { get; set; }
  public Matrix<double>? ExpectedRotationMatrix { get; set; }
  public Matrix<double>? ExpectedTransformationMatrix { get; set; }
  public Matrix<double>? ExpectedLocalStiffnessMatrix { get; set; }
  public Matrix<double>? ExpectedGlobalStiffnessMatrix { get; set; }
  public Vector<double>? ExpectedLocalFixedEndForces { get; set; }
  public Vector<double>? ExpectedGlobalFixedEndForces { get; set; }
  public Vector<double>? ExpectedLocalEndDisplacements { get; set; }
  public Vector<double>? ExpectedGlobalEndDisplacements { get; set; }
  public Vector<double>? ExpectedLocalEndForces { get; set; }
  public Vector<double>? ExpectedGlobalEndForces { get; set; }
}

public interface IHasLocalResults
{
  public Matrix<double>? ExpectedRotationMatrix { get; set; }
  public Matrix<double>? ExpectedTransformationMatrix { get; set; }
  public Matrix<double>? ExpectedLocalStiffnessMatrix { get; set; }
  public Vector<double>? ExpectedLocalFixedEndForces { get; set; }
  public Vector<double>? ExpectedLocalEndDisplacements { get; set; }
  public Vector<double>? ExpectedLocalEndForces { get; set; }
}

public interface IHasGlobalResults
{
  public Matrix<double>? ExpectedGlobalStiffnessMatrix { get; set; }
  public Vector<double>? ExpectedGlobalFixedEndForces { get; set; }
  public Vector<double>? ExpectedGlobalEndDisplacements { get; set; }
  public Vector<double>? ExpectedGlobalEndForces { get; set; }
}
