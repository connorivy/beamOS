using beamOS.API.Schema.Objects;
using beamOS.Tests.TestObjects.Element1Ds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beamOS.Tests.TestObjects
{
  internal class SolvedProblem
  {
    public SolvedProblem() { }
    public virtual AnalyticalModelFixture AnalyticalModelFixture { get; set; }
    public List<Element1DFixture> Element1DFixtures { get; set; }
  }
}
