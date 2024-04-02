//using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate;
using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalElement1Ds;
using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalModels;
using BeamOs.Domain.DirectStiffnessMethod.DsmNodeAggregate;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.SolvedProblems;

public abstract class SolvedProblem
{
    public AnalyticalModelFixture AnalyticalModelFixture { get; set; }
    public List<AnalyticalElement1DFixture> Element1DFixtures { get; set; } = [];
    public List<DsmNode> AnalyticalNodes { get; set; } = [];
}
