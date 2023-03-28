using beamOS.Tests.TestObjects.AnalyticalModels;
using beamOS.Tests.TestObjects.Element1Ds;

namespace beamOS.Tests.TestObjects.SolvedProblems
{
    public abstract class SolvedProblem
    {
        public SolvedProblem() { }
        public abstract AnalyticalModelFixture AnalyticalModelFixture { get; set; }
        public List<Element1DFixture> Element1DFixtures { get; set; } = new List<Element1DFixture>();
    }
}
