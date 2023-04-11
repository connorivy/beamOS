using beamOS.API.Schema.Objects;
using beamOS.Tests.TestObjects.SolvedProblems;
using LanguageExt;
using MathNet.Numerics.LinearAlgebra;

namespace beamOS.Tests.TestObjects.Element1Ds
{

  public class AllElement1DFixtures : TheoryData<Element1DFixture>
  {
    public List<Element1DFixture> Element1DFixtures = new()
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
          Add(element1DFixture);
        }
      }

      foreach (var element1DFixture in Element1DFixtures)
      {
        Add(element1DFixture);
      }
    }
  }

  public class Element1DFixture : SerializableFixtureBase, IHasLocalResults, IHasGlobalResults
  {
    public Element1DFixture()
    {
    }
    public Element1DFixture(Element1D element)
    {
      Element = element;
    }
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

  public interface IHasLocalResults
  {
    public Option<Matrix<double>> ExpectedRotationMatrix { get; set; }
    public Option<Matrix<double>> ExpectedTransformationMatrix { get; set; }
    public Option<Matrix<double>> ExpectedLocalStiffnessMatrix { get; set; }
    public Option<Vector<double>> ExpectedLocalFixedEndForces { get; set; }
    public Option<Vector<double>> ExpectedLocalEndDisplacements { get; set; }
    public Option<Vector<double>> ExpectedLocalEndForces { get; set; }
  }

  public interface IHasGlobalResults
  {
    public Option<Matrix<double>> ExpectedGlobalStiffnessMatrix { get; set; }
    public Option<Vector<double>> ExpectedGlobalFixedEndForces { get; set; }
    public Option<Vector<double>> ExpectedGlobalEndDisplacements { get; set; }
    public Option<Vector<double>> ExpectedGlobalEndForces { get; set; }
  }
}
