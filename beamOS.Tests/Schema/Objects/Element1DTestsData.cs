﻿using beamOS.API.Schema.Objects;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beamOS.Tests.Schema.Objects
{
  public class Element1DTestsData
  {
    #region TestGetRotationMatrixData

    #region VerticalMembers
    public static IEnumerable<object[]> TestGetRotationMatrixData_VerticalMembers()
    {
      var tests = new List<object[]>();
      double rotation;

      // by default (aka no profile rotation), a vertical member will end up with it's local
      // y axis aligned in the global -x direction
      tests.Add(new object[]
      {
        new Line(new[] { 10.0, 10, 5 }, new[] { 10.0, 18, 5 }),
        0,
        new[,]
        {
          { 0, 1.0, 0 },
          { -1.0, 0, 0 },
          { 0, 0, 1.0 },
        }
      });

      // a positive (counter clockwise) 90 degree rotation will align the local y axis in the global +z direction
      tests.Add(new object[]
      {
        new Line(new[] { -9.0, -7, 5 }, new[] { -9.0, 0, 5 }),
        Math.PI / 2,
        new[,]
        {
          { 0, 1.0, 0 },
          { 0, 0, 1.0 },
          { 1.0, 0, 0 },
        }
      });

      // a negative (clockwise) 30 degree rotation will rotate the local y axis 30 degrees counter clockwise from the global -x direction
      // the local y axis will be -cos(-30d) in the global x direction and sin(-30d) in the global z dir
      // the local z axis will be sin(-30d) in the global x direction and cos(-30d) in the global z dir
      rotation = -Math.PI / 6;
      tests.Add(new object[]
      {
        new Line(new[] { 9.0, -7, 2.5 }, new[] { 9.0, 5, 2.5 }),
        rotation, // 30 degrees
        new[,]
        {
          { 0, 1.0, 0 },
          // -1.0 * cos(-30d), 0, sin(-30d)
          { -0.86602540378, 0, -.5 },
          // 1.0 * sin(-30d), 0, cos(-30d)
          { -.5, 0, 0.86602540378 },
        }
      });

      // a vertical member that has point 0 above point 1 will be aligned in the global -x dir
      // the local y axis will be in the global +x dir and the local z will be in the global +z
      tests.Add(new object[]
      {
        new Line(new[] { -9.0, 7, 5 }, new[] { -9.0, 0, 5 }),
        0,
        new[,]
        {
          { 0, -1.0, 0 },
          { 1.0, 0, 0 },
          { 0, 0, 1.0 },
        }
      });

      // for an element aligned with the global -y axis,
      // a positive (counter clockwise) 90 degree rotation will align the local y axis in the global +z direction
      tests.Add(new object[]
      {
        new Line(new[] { 10.0, 10, 5 }, new[] { 10.0, -18, 5 }),
        Math.PI / 2,
        new[,]
        {
          { 0, -1.0, 0 },
          { 0, 0, 1.0 },
          { -1.0, 0, 0 },
        }
      });

      rotation = Math.PI / 20;
      tests.Add(new object[]
      {
        new Line(new[] { 10.0, 10, 5 }, new[] { 10.0, -18, 5 }),
        rotation,
        new[,]
        {
          { 0, -1.0, 0 },
          { -(-1.0) * Math.Cos(rotation), 0, Math.Sin(rotation) },
          { (-1.0) * Math.Sin(rotation), 0, Math.Cos(rotation) },
        }
      });

      rotation = - 2 * Math.PI / 5;
      tests.Add(new object[]
      {
        new Line(new[] { 10.0, 10, 5 }, new[] { 10.0, -18, 5 }),
        rotation,
        new[,]
        {
          { 0, -1.0, 0 },
          { -(-1.0) * Math.Cos(rotation), 0, Math.Sin(rotation) },
          { (-1.0) * Math.Sin(rotation), 0, Math.Cos(rotation) },
        }
      });

      return tests;
    }
    #endregion

    #region AlignedWithGlobalPlanes
    public static IEnumerable<object[]> TestGetRotationMatrixData()
    {
      var tests = new List<object[]>();

      // if the beam is oriented in the same direction as the global coordinate system,
      // then the unit vectors of the global domain should be returned
      tests.Add(new object[]
      {
        new Line(new[] { 10.0, 7, -3 }, new[] { 20.0, 7, -3 }),
        0,
        new[,]
        {
          { 1.0, 0, 0},
          { 0, 1.0, 0},
          { 0, 0, 1.0},
        }
      });

      // if an elements local xy plane is equal to or parallel with the global xy plane,
      // return the following matrix (ref Advanced Structural Analysis with MATLAB eqn 4.17)
      var curve = new Line(new[] { 10.0, 10, 5 }, new[] { 20.0, 18, 5 });
      var L = curve.Length;
      var cx = (curve.P1[0] - curve.P0[0]) / L;
      var cy = (curve.P1[1] - curve.P0[1]) / L;
      var cz = (curve.P1[2] - curve.P0[2]) / L;
      var sqrtCx2Cz2 = Math.Sqrt(Math.Pow(cx, 2) + Math.Pow(cz, 2));
      tests.Add(new object[]
      {
        curve,
        0,
        new[,]
        {
          { sqrtCx2Cz2, cy, 0},
          { -cy, sqrtCx2Cz2, 0},
          { 0, 0, 1.0},
        }
      });

      curve = new Line(new[] { -10.0, 10, 5 }, new[] { 20.0, 18, 5 });
      L = curve.Length;
      cx = (curve.P1[0] - curve.P0[0]) / L;
      cy = (curve.P1[1] - curve.P0[1]) / L;
      cz = (curve.P1[2] - curve.P0[2]) / L;
      sqrtCx2Cz2 = Math.Sqrt(Math.Pow(cx, 2) + Math.Pow(cz, 2));
      tests.Add(new object[]
      {
        curve,
        0,
        new[,]
        {
          { sqrtCx2Cz2, cy, 0},
          { -cy, sqrtCx2Cz2, 0},
          { 0, 0, 1.0},
        }
      });

      // if an elements local xz is equal to or parallel with the global xz plane,
      // return the following matrix (ref Advanced Structural Analysis with MATLAB eqn 4.16)
      curve = new Line(new[] { 10.0, 18, 15 }, new[] { 20.0, 18, 5 });
      L = curve.Length;
      cx = (curve.P1[0] - curve.P0[0]) / L;
      //cy = (curve.P1[1] - curve.P0[1]) / L;
      cz = (curve.P1[2] - curve.P0[2]) / L;
      sqrtCx2Cz2 = Math.Sqrt(Math.Pow(cx, 2) + Math.Pow(cz, 2));
      tests.Add(new object[]
      {
        curve,
        0,
        new[,]
        {
          { cx / sqrtCx2Cz2, 0, cz / sqrtCx2Cz2},
          { 0, 1.0, 0},
          { - cz / sqrtCx2Cz2, 0, cx / sqrtCx2Cz2},
        }
      });

      curve = new Line(new[] { -10.0, 18, 15 }, new[] { 20.0, 18, 5 });
      L = curve.Length;
      cx = (curve.P1[0] - curve.P0[0]) / L;
      //cy = (curve.P1[1] - curve.P0[1]) / L;
      cz = (curve.P1[2] - curve.P0[2]) / L;
      sqrtCx2Cz2 = Math.Sqrt(Math.Pow(cx, 2) + Math.Pow(cz, 2));
      tests.Add(new object[]
      {
        curve,
        0,
        new[,]
        {
          { cx / sqrtCx2Cz2, 0, cz / sqrtCx2Cz2},
          { 0, 1.0, 0},
          { - cz / sqrtCx2Cz2, 0, cx / sqrtCx2Cz2},
        }
      });

      // if an element is aligned with the global coord system, but has a non 0 rotation,
      // return the following matrix (ref Advanced Structural Analysis with MATLAB eqn 4.18)
      curve = new Line(new[] { 10.0, 18, -15 }, new[] { 20.0, 18, -15 });
      var rotation = Math.PI / 2; // 90 degree profile rotation
      tests.Add(new object[]
      {
        curve,
        rotation,
        new[,]
        {
          { 1.0, 0, 0},
          { 0, Math.Cos(rotation), Math.Sin(rotation)},
          { 0, -Math.Sin(rotation), Math.Cos(rotation)},
        }
      });

      curve = new Line(new[] { 10.0, 18, -15 }, new[] { 20.0, 18, -15 });
      rotation = - Math.PI / 5; // 36 degrees clockwise profile rotation
      tests.Add(new object[]
      {
        curve,
        rotation,
        new[,]
        {
          { 1.0, 0, 0},
          { 0, Math.Cos(rotation), Math.Sin(rotation)},
          { 0, -Math.Sin(rotation), Math.Cos(rotation)},
        }
      });

      return tests;
    }
    #endregion

    #region misaligned
    public static IEnumerable<object[]> TestGetRotationMatrixData_Misaligned()
    {
      //var tests = new List<object[]>();

      // simplest case
      yield return new object[]
      {
        new Line(new[] { 0.0, 0.0, 0.0 }, new[] { 1.0, 1, 1 }),
        0,
        new[,]
        {
          { 0.57735026919, 0.57735026919, 0.57735026919},
          { -0.408248,  0.816497,  -0.408248},
          { -0.70710678118, 0, 0.70710678118},
        }
      };

      // From Matrix Analysis of Structures example 8.3
      yield return new object[]
      {
        new Line(new[] { 4.0, 7.0, 6.0 }, new[] { 20.0, 15, 17 }),
        0.857302717,
        new[,]
        {
          { 0.7619, 0.38095, 0.52381},
          { -0.6338, 0.60512, 0.48181},
          { -0.13343, -0.69909, 0.70249},
        }
      };


    }
    #endregion

    #endregion
  }
}
