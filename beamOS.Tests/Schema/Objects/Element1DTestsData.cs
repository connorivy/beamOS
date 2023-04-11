namespace beamOS.Tests.Schema.Objects;

using beamOS.API.Schema.Objects;

public class Element1DTestsData
{
  public static IEnumerable<object[]> TestGetRotationMatrixData()
  {
    var tests = new List<object[]>();
    double rotation;

    #region AlignedWithGlobalPlanes

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
#pragma warning disable IDE1006 // Naming Styles
    var L = curve.Length;
#pragma warning restore IDE1006 // Naming Styles
    var cx = (curve.EndNode1.Position[0] - curve.EndNode0.Position[0]) / L;
    var cy = (curve.EndNode1.Position[1] - curve.EndNode0.Position[1]) / L;
    var cz = (curve.EndNode1.Position[2] - curve.EndNode0.Position[2]) / L;
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
    cx = (curve.EndNode1.Position[0] - curve.EndNode0.Position[0]) / L;
    cy = (curve.EndNode1.Position[1] - curve.EndNode0.Position[1]) / L;
    cz = (curve.EndNode1.Position[2] - curve.EndNode0.Position[2]) / L;
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
    cx = (curve.EndNode1.Position[0] - curve.EndNode0.Position[0]) / L;
    //cy = (curve.EndNode1.Position[1] - curve.EndNode0.Position[1]) / L;
    cz = (curve.EndNode1.Position[2] - curve.EndNode0.Position[2]) / L;
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
    cx = (curve.EndNode1.Position[0] - curve.EndNode0.Position[0]) / L;
    //cy = (curve.EndNode1.Position[1] - curve.EndNode0.Position[1]) / L;
    cz = (curve.EndNode1.Position[2] - curve.EndNode0.Position[2]) / L;
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
    rotation = Math.PI / 2; // 90 degree profile rotation
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
    rotation = -Math.PI / 5; // 36 degrees clockwise profile rotation
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

    #endregion

    #region VerticalMembers
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

    rotation = -2 * Math.PI / 5;
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

    #endregion

    #region misaligned
    // simplest case
    tests.Add(new object[]
    {
    new Line(new[] { 0.0, 0.0, 0.0 }, new[] { 1.0, 1, 1 }),
    0,
    new[,]
    {
      { 0.57735026919, 0.57735026919, 0.57735026919},
      { -0.408248,  0.816497,  -0.408248},
      { -0.70710678118, 0, 0.70710678118},
    }
    });

    // From Matrix Analysis of Structures example 8.3
    tests.Add(new object[]
    {
    new Line(new[] { 4.0, 7.0, 6.0 }, new[] { 20.0, 15, 17 }),
    0.857302717,
    new[,]
    {
      { 0.7619, 0.38095, 0.52381},
      { -0.6338, 0.60512, 0.48181},
      { -0.13343, -0.69909, 0.70249},
    }
    });
    #endregion

    return tests;
  }

  public static IEnumerable<object[]> TestGetTransformationMatrixData()
  {
    var tests = new List<object[]>
    {
      new object[]
    {
      new [,]
      {
        { 1.0, 1, 1 },
        { 1, 1, 1 },
        { 1, 1, 1 },
        { 1, 1, 1 },
      },
      true
    },
      new object[]
    {
      new [,]
      {
        { 0.57735026919, 0.57735026919, 0.57735026919, 0},
        { -0.408248,  0.816497,  -0.408248, 0},
        { -0.70710678118, 0, 0.70710678118, 0},
      },
      true
    },

      new object[]
    {
      new [,]
      {
        { 1.0, 1, 1 },
        { 1, 1, 1 },
        { 1, 1, 1 },
      },
    },

      new object[]
    {
      new [,]
      {
        { 0.57735026919, 0.57735026919, 0.57735026919},
        { -0.408248,  0.816497,  -0.408248},
        { -0.70710678118, 0, 0.70710678118},
      },
    }
    };

    return tests;
  }

  public static IEnumerable<object[]> TestGetLocalStiffnessMatrixData()
  {
    var tests = new List<object[]>
    {
      #region nullValues
      new object[]
    {
      // E, G, A, Iz, Iy, J, P0, P1, matrixArray, execptionThrown
      null, 0, null, 0, 0, 0, new[] {0.0,0,0}, new[] {1.0,0,0}, new double[12,12], true
    },

      new object[]
    {
      // E, G, A, Iz, Iy, J, P0, P1, matrixArray, execptionThrown
      null, 0, 0, 0, 0, 0, new[] {0.0,0,0}, new[] {1.0,0,0}, new double[12,12], true
    },

      new object[]
    {
      // E, G, A, Iz, Iy, J, P0, P1, matrixArray, execptionThrown
      0, 0, null, 0, 0, 0, new[] {0.0,0,0}, new[] {1.0,0,0}, new double[12,12], true
    },
      #endregion

      // all values are one which should lead to just the coefficient matrix
      new object[]
    {
      // E, G, A, Iz, Iy, J, P0, P1, matrixArray, execptionThrown
      1, 1, 1, 1, 1, 1, new[] {0.0,0,0}, new[] {1.0,0,0}, new double[12,12]
      {
        {  1,   0,   0,  0,  0,  0, -1,   0,   0,  0,  0,  0 },
        {  0,  12,   0,  0,  0,  6,  0, -12,   0,  0,  0,  6 },
        {  0,   0,  12,  0, -6,  0,  0,   0, -12,  0, -6,  0 },
        {  0,   0,   0,  1,  0,  0,  0,   0,   0, -1,  0,  0 },
        {  0,   0,  -6,  0,  4,  0,  0,   0,   6,  0,  2,  0 },
        {  0,   6,   0,  0,  0,  4,  0,  -6,   0,  0,  0,  2 },
        { -1,   0,   0,  0,  0,  0,  1,   0,   0,  0,  0,  0 },
        {  0, -12,   0,  0,  0, -6,  0,  12,   0,  0,  0, -6 },
        {  0,   0, -12,  0,  6,  0,  0,   0,  12,  0,  6,  0 },
        {  0,   0,   0, -1,  0,  0,  0,   0,   0,  1,  0,  0 },
        {  0,   0,  -6,  0,  2,  0,  0,   0,   6,  0,  4,  0 },
        {  0,   6,   0,  0,  0,  2,  0,  -6,   0,  0,  0,  4 },
      },
      false
    },

      // isolate the E variable
      new object[]
    {
      // E, G, A, Iz, Iy, J, P0, P1, matrixArray, execptionThrown
      5, 1, 1, 1, 1, 1, new[] {0.0,0,0}, new[] {1.0,0,0}, new double[12,12]
      {
        {  5*1,   0,   0,  0,  0,  0, -5*1,   0,   0,  0,  0,  0 },
        {  0,  5*12,   0,  0,  0,  5*6,  0, -5*12,   0,  0,  0,  5*6 },
        {  0,   0,  5*12,  0, -5*6,  0,  0,   0, -5*12,  0, -5*6,  0 },
        {  0,   0,   0,  1,  0,  0,  0,   0,   0, -1,  0,  0 },
        {  0,   0,  -5*6,  0,  5*4,  0,  0,   0,   5*6,  0,  5*2,  0 },
        {  0,   5*6,   0,  0,  0,  5*4,  0,  -5*6,   0,  0,  0,  5*2 },
        { -5*1,   0,   0,  0,  0,  0,  5*1,   0,   0,  0,  0,  0 },
        {  0, -5*12,   0,  0,  0, -5*6,  0,  5*12,   0,  0,  0, -5*6 },
        {  0,   0, -5*12,  0,  5*6,  0,  0,   0,  5*12,  0,  5*6,  0 },
        {  0,   0,   0, -1,  0,  0,  0,   0,   0,  1,  0,  0 },
        {  0,   0,  -5*6,  0,  5*2,  0,  0,   0,   5*6,  0,  5*4,  0 },
        {  0,   5*6,   0,  0,  0,  5*2,  0,  -5*6,   0,  0,  0,  5*4 },
      },
      false
    },

      // isolate the G variable
      new object[]
    {
      // E, G, A, Iz, Iy, J, P0, P1, matrixArray, execptionThrown
      1, 5, 1, 1, 1, 1, new[] {0.0,0,0}, new[] {1.0,0,0}, new double[12,12]
      {
        {  1,   0,   0,  0,  0,  0, -1,   0,   0,  0,  0,  0 },
        {  0,  12,   0,  0,  0,  6,  0, -12,   0,  0,  0,  6 },
        {  0,   0,  12,  0, -6,  0,  0,   0, -12,  0, -6,  0 },
        {  0,   0,   0,  5*1,  0,  0,  0,   0,   0, -5*1,  0,  0 },
        {  0,   0,  -6,  0,  4,  0,  0,   0,   6,  0,  2,  0 },
        {  0,   6,   0,  0,  0,  4,  0,  -6,   0,  0,  0,  2 },
        { -1,   0,   0,  0,  0,  0,  1,   0,   0,  0,  0,  0 },
        {  0, -12,   0,  0,  0, -6,  0,  12,   0,  0,  0, -6 },
        {  0,   0, -12,  0,  6,  0,  0,   0,  12,  0,  6,  0 },
        {  0,   0,   0, -5*1,  0,  0,  0,   0,   0,  5*1,  0,  0 },
        {  0,   0,  -6,  0,  2,  0,  0,   0,   6,  0,  4,  0 },
        {  0,   6,   0,  0,  0,  2,  0,  -6,   0,  0,  0,  4 },
      },
      false
    },

      // isolate the A variable
      new object[]
    {
      // E, G, A, Iz, Iy, J, P0, P1, matrixArray, execptionThrown
      1, 1, 5, 1, 1, 1, new[] {0.0,0,0}, new[] {1.0,0,0}, new double[12,12]
      {
        {  5*1,   0,   0,  0,  0,  0, -5*1,   0,   0,  0,  0,  0 },
        {  0,  12,   0,  0,  0,  6,  0, -12,   0,  0,  0,  6 },
        {  0,   0,  12,  0, -6,  0,  0,   0, -12,  0, -6,  0 },
        {  0,   0,   0,  1,  0,  0,  0,   0,   0, -1,  0,  0 },
        {  0,   0,  -6,  0,  4,  0,  0,   0,   6,  0,  2,  0 },
        {  0,   6,   0,  0,  0,  4,  0,  -6,   0,  0,  0,  2 },
        { -5*1,   0,   0,  0,  0,  0,  5*1,   0,   0,  0,  0,  0 },
        {  0, -12,   0,  0,  0, -6,  0,  12,   0,  0,  0, -6 },
        {  0,   0, -12,  0,  6,  0,  0,   0,  12,  0,  6,  0 },
        {  0,   0,   0, -1,  0,  0,  0,   0,   0,  1,  0,  0 },
        {  0,   0,  -6,  0,  2,  0,  0,   0,   6,  0,  4,  0 },
        {  0,   6,   0,  0,  0,  2,  0,  -6,   0,  0,  0,  4 },
      },
      false
    },

      // isolate the Iz variable
      new object[]
    {
      // E, G, A, Iz, Iy, J, P0, P1, matrixArray, execptionThrown
      1, 1, 1, 5, 1, 1, new[] {0.0,0,0}, new[] {1.0,0,0}, new double[12,12]
      {
        {  1,   0,   0,  0,  0,  0, -1,   0,   0,  0,  0,  0 },
        {  0,  5*12,   0,  0,  0,  5*6,  0, -5*12,   0,  0,  0,  5*6 },
        {  0,   0,  12,  0, -6,  0,  0,   0, -12,  0, -6,  0 },
        {  0,   0,   0,  1,  0,  0,  0,   0,   0, -1,  0,  0 },
        {  0,   0,  -6,  0,  4,  0,  0,   0,   6,  0,  2,  0 },
        {  0,   5*6,   0,  0,  0,  5*4,  0,  -5*6,   0,  0,  0,  5*2 },
        { -1,   0,   0,  0,  0,  0,  1,   0,   0,  0,  0,  0 },
        {  0, -5*12,   0,  0,  0, -5*6,  0,  5*12,   0,  0,  0, -5*6 },
        {  0,   0, -12,  0,  6,  0,  0,   0,  12,  0,  6,  0 },
        {  0,   0,   0, -1,  0,  0,  0,   0,   0,  1,  0,  0 },
        {  0,   0,  -6,  0,  2,  0,  0,   0,   6,  0,  4,  0 },
        {  0,   5*6,   0,  0,  0,  5*2,  0,  -5*6,   0,  0,  0,  5*4 },
      },
      false
    },

      // isolate the Iy variable
      new object[]
    {
      // E, G, A, Iz, Iy, J, P0, P1, matrixArray, execptionThrown
      1, 1, 1, 1, 5, 1, new[] {0.0,0,0}, new[] {1.0,0,0}, new double[12,12]
      {
        {  1,   0,   0,  0,  0,  0, -1,   0,   0,  0,  0,  0 },
        {  0,  12,   0,  0,  0,  6,  0, -12,   0,  0,  0,  6 },
        {  0,   0,  5*12,  0, -5*6,  0,  0,   0, -5*12,  0, -5*6,  0 },
        {  0,   0,   0,  1,  0,  0,  0,   0,   0, -1,  0,  0 },
        {  0,   0,  -5*6,  0,  5*4,  0,  0,   0,   5*6,  0,  5*2,  0 },
        {  0,   6,   0,  0,  0,  4,  0,  -6,   0,  0,  0,  2 },
        { -1,   0,   0,  0,  0,  0,  1,   0,   0,  0,  0,  0 },
        {  0, -12,   0,  0,  0, -6,  0,  12,   0,  0,  0, -6 },
        {  0,   0, -5*12,  0,  5*6,  0,  0,   0,  5*12,  0,  5*6,  0 },
        {  0,   0,   0, -1,  0,  0,  0,   0,   0,  1,  0,  0 },
        {  0,   0,  -5*6,  0,  5*2,  0,  0,   0,   5*6,  0,  5*4,  0 },
        {  0,   6,   0,  0,  0,  2,  0,  -6,   0,  0,  0,  4 },
      },
      false
    },

      // isolate the J variable
      new object[]
    {
      // E, G, A, Iz, Iy, J, P0, P1, matrixArray, execptionThrown
      1, 1, 1, 1, 1, 5, new[] {0.0,0,0}, new[] {1.0,0,0}, new double[12,12]
      {
        {  1,   0,   0,  0,  0,  0, -1,   0,   0,  0,  0,  0 },
        {  0,  12,   0,  0,  0,  6,  0, -12,   0,  0,  0,  6 },
        {  0,   0,  12,  0, -6,  0,  0,   0, -12,  0, -6,  0 },
        {  0,   0,   0,  5*1,  0,  0,  0,   0,   0, -5*1,  0,  0 },
        {  0,   0,  -6,  0,  4,  0,  0,   0,   6,  0,  2,  0 },
        {  0,   6,   0,  0,  0,  4,  0,  -6,   0,  0,  0,  2 },
        { -1,   0,   0,  0,  0,  0,  1,   0,   0,  0,  0,  0 },
        {  0, -12,   0,  0,  0, -6,  0,  12,   0,  0,  0, -6 },
        {  0,   0, -12,  0,  6,  0,  0,   0,  12,  0,  6,  0 },
        {  0,   0,   0, -5*1,  0,  0,  0,   0,   0,  5*1,  0,  0 },
        {  0,   0,  -6,  0,  2,  0,  0,   0,   6,  0,  4,  0 },
        {  0,   6,   0,  0,  0,  2,  0,  -6,   0,  0,  0,  4 },
      },
      false
    }
    };

    // isolate the L variable
#pragma warning disable IDE1006 // Naming Styles
    var L = 5.0;
    var L2 = 5.0 * 5;
    var L3 = 5.0 * 5 * 5;
#pragma warning restore IDE1006 // Naming Styles
    tests.Add(new object[]
    {
      // E, G, A, Iz, Iy, J, P0, P1, matrixArray, execptionThrown
      1, 1, 1, 1, 1, 1, new[] {0.0,0,0}, new[] {5.0,0,0}, new double[12,12]
      {
        {  1/L,   0,   0,  0,  0,  0, -1/L,   0,   0,  0,  0,  0 },
        {  0,  12/L3,   0,  0,  0,  6/L2,  0, -12/L3,   0,  0,  0,  6/L2 },
        {  0,   0,  12/L3,  0, -6/L2,  0,  0,   0, -12/L3,  0, -6/L2,  0 },
        {  0,   0,   0,  1/L,  0,  0,  0,   0,   0, -1/L,  0,  0 },
        {  0,   0,  -6/L2,  0,  4/L,  0,  0,   0,   6/L2,  0,  2/L,  0 },
        {  0,   6/L2,   0,  0,  0,  4/L,  0,  -6/L2,   0,  0,  0,  2/L },
        { -1/L,   0,   0,  0,  0,  0,  1/L,   0,   0,  0,  0,  0 },
        {  0, -12/L3,   0,  0,  0, -6/L2,  0,  12/L3,   0,  0,  0, -6/L2 },
        {  0,   0, -12/L3,  0,  6/L2,  0,  0,   0,  12/L3,  0,  6/L2,  0 },
        {  0,   0,   0, -1/L,  0,  0,  0,   0,   0,  1/L,  0,  0 },
        {  0,   0,  -6/L2,  0,  2/L,  0,  0,   0,   6/L2,  0,  4/L,  0 },
        {  0,   6/L2,   0,  0,  0,  2/L,  0,  -6/L2,   0,  0,  0,  4/L },
      },
      false
    });

    return tests;
  }
}
