using beamOS.API.Schema.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beamOS.Tests.Schema.Objects
{
  public partial class AnalyticalModelTestsData
  {
    public static IEnumerable<object[]> TestExpandOctreeData()
    {
      var tests = new List<object[]>();

      tests.Add(new object[]
      {
        new double[4][] {
          new double[] { 0.0, 0, 0 },
          new double[] { -20, 0, 0 },
          new double[] { 0, -20, 0 },
          new double[] { 0, 0, -20 }
        },
        //new bool[4][]
        //{
        //  new bool[] {true, true, true, true, true, true},
        //  new bool[] {false, false, false, false, false, false},
        //  new bool[] {false, false, false, false, false, false},
        //  new bool[] {false, false, false, false, false, false}
        //},
        new int[24][]
        {
          new int[] {1, 0},
          new int[] {1, 1},
          new int[] {1, 2},
          new int[] {1, 3},
          new int[] {1, 4},
          new int[] {1, 5},
          new int[] {2, 0},
          new int[] {2, 1},
          new int[] {2, 2},
          new int[] {2, 3},
          new int[] {2, 4},
          new int[] {2, 5},
          new int[] {3, 0},
          new int[] {3, 1},
          new int[] {3, 2},
          new int[] {3, 3},
          new int[] {3, 4},
          new int[] {3, 5},
          new int[] {0, 0},
          new int[] {0, 1},
          new int[] {0, 2},
          new int[] {0, 3},
          new int[] {0, 4},
          new int[] {0, 5}
        }
      });

      return tests;
    }
  }
}
