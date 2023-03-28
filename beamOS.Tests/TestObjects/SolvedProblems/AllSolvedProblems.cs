using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beamOS.Tests.TestObjects.SolvedProblems
{
    public class AllSolvedProblems : TheoryData<SolvedProblem>
    {
        public List<SolvedProblem> SolvedProblems = new()
    {
      new MatrixAnalysisOfStructures_2ndEd.Example8_4()
    };
        public AllSolvedProblems()
        {
            foreach (var solvedProblem in SolvedProblems)
            {
                Add(solvedProblem);
            }
        }
    }

    [CollectionDefinition("Solved Problems")]
    public class DatabaseCollection : ICollectionFixture<AllSolvedProblems>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
