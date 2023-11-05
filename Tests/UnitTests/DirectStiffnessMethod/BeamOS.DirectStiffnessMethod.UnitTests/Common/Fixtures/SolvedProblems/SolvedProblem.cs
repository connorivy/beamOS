using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalElement1Ds;
using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalModels;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.SolvedProblems;

public abstract class SolvedProblem
{
    public SolvedProblem() { }
    public abstract AnalyticalModelFixture AnalyticalModelFixture { get; set; }
    public List<AnalyticalElement1DFixture> Element1DFixtures { get; set; } = new();
}
