﻿using beamOS.API.Schema.Objects;
using beamOS.Tests.TestObjects.AnalyticalModels;
using beamOS.Tests.TestObjects.Element1Ds;
using LanguageExt;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
//using Element1DFixtures = beamOS.Tests.TestObjects.Element1Ds.MatrixAnalysisOfStructures_2ndEd.Example8_4;

namespace beamOS.Tests.TestObjects.MatrixAnalysisOfStructures_2ndEd
{
  internal class Example8_4 : SolvedProblem
  {
    public override AnalyticalModelFixture AnalyticalModelFixture { get; set; }
    public Example8_4()
    {
      Element1DFixtures.Add(Element1);
      Element1DFixtures.Add(Element2);
      Element1DFixtures.Add(Element3);

      AnalyticalModelFixture = ModelFixture;
    }

    #region AnalyticalModelFixtureDefinition
    private static Option<AnalyticalModelFixture> _analyticalModelFixture;
    public static AnalyticalModelFixture ModelFixture => _analyticalModelFixture
      .Match(
        m => m,
        () =>
        {
          var m = GetAnalyticalModel();
          _analyticalModelFixture = m;
          return m;
        }
      );

    public static AnalyticalModelFixture GetAnalyticalModel()
    {
      #region ModelDefinition
      var am = new AnalyticalModel(Element1.Element.BaseCurve.EndNode0.Position);

      am.AddElement1D(Element1.Element);
      am.AddElement1D(Element2.Element);
      am.AddElement1D(Element3.Element);

      #endregion

      #region ResultsDefinition

      #endregion

      return new AnalyticalModelFixture(am)
      {

      };
    }
    #endregion

    #region Element1DFixtureDefinitions
    //private static Element1DFixture Element1Fixture => _element1Fixture.Value;
    //private static readonly Lazy<Element1DFixture> _element1Fixture = new(GetElement1Fixture);
    private static Option<Element1DFixture> _element1;
    public static Element1DFixture Element1 => _element1
      .Match(
        el => el,
        () =>
        {
          var el = GetElement1Fixture();
          _element1 = el;
          return el;
        }
      );
    private static Option<Element1DFixture> _element2;
    public static Element1DFixture Element2 => _element2
      .Match(
        el => el,
        () =>
        {
          var el = GetElement2Fixture();
          _element2 = el;
          return el;
        }
      );
    private static Option<Element1DFixture> _element3;
    public static Element1DFixture Element3 => _element3
      .Match(
        el => el,
        () =>
        {
          var el = GetElement3Fixture();
          _element3 = el;
          return el;
        }
      );

    private static Element1DFixture GetElement1Fixture()
    {
      #region ElementDefinition
      var baseLine = new Line(new double[] { -20 * 12, 0, 0 }, new double[] { 0, 0, 0 });
      var section = new SectionProfile()
      {
        A = 32.9,
        Iz = 716,
        Iy = 236,
        J = 15.1
      };
      var material = new Material()
      {
        E = 29000,
        G = 11500
      };
      var element = new Element1D(baseLine, section, material, ElementType.Beam);
      #endregion

      #region ResultsDefinition
      var rotationMatrix = DenseMatrix.OfArray(new double[,]
      {
        { 1, 0, 0 },
        { 0, 1, 0 },
        { 0, 0, 1 }
      });

      var transformationMatrix = DenseMatrix.OfArray(new double[,]
      {
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }
      });

      var localStiffnessMatrix = DenseMatrix.OfArray(new double[,] {
        { 3975.4,  0,        0,        0,       0,        0,        -3975.4,  0,        0,        0,        0,        0       },
        { 0,       18.024,   0,        0,       0,        2162.9,   0,        -18.024,  0,        0,        0,        2162.9  },
        { 0,       0,        5.941,    0,       -712.92,  0,        0,        0,        -5.941,   0,        -712.92,  0,      },
        { 0,       0,        0,        723.54,  0,        0,        0,        0,        0,        -723.54,  0,        0       },
        { 0,       0,        -712.92,  0,       114067,   0,        0,        0,        712.92,   0,        57033,    0       },
        { 0,       2162.9,   0,        0,       0,        346067,   0,        -2162.9,  0,        0,        0,        173033  },
        { -3975.4, 0,        0,        0,       0,        0,        3975.4,   0,        0,        0,        0,        0       },
        { 0,       -18.024,  0,        0,       0,        -2162.9,  0,        18.024,   0,        0,        0,        -2162.9 },
        { 0,       0,        -5.941,   0,       712.92,   0,        0,        0,        5.941,    0,        712.92,   0       },
        { 0,       0,        -723.54,  0,       0,        0,        0,        0,        723.54,   0,        0,        0       },
        { 0,       0,        -712.92,  0,       57033,    0,        0,        0,        712.92,   0,        114067,   0       },
        { 0,       2162.9,   0,        0,       0,        173033,   0,        -2162.9,  0,        0,        0,        346067  }
      });

      var localfixedEndForces = Vector<double>.Build.Dense(new double[]
      {
        0,
        30,
        0,
        0,
        0,
        12000,
        0,
        30,
        0,
        0,
        0,
        -1200
      });

      var localEndDisplacements = Vector<double>.Build.Dense(new double[]
      {
        0,
        0,
        0,
        0,
        0,
        0,
        -1.3522,
        -2.7965,
        -1.812,
        -3.0021,
        1.0569,
        6.4986
      }) * Math.Pow(10, -3);

      var localEndForces = Vector<double>.Build.Dense(new double[]
      {
        5.3757,
        44.106,
        -0.74272,
        2.1722,
        58.987,
        2330.5,
        -5.3757,
        15.894,
        0.74272,
        -2.1722,
        119.27,
        1055
      });
      #endregion

      return new Element1DFixture(element)
      {
        ExpectedRotationMatrix = rotationMatrix,
        ExpectedTransformationMatrix = transformationMatrix,
        ExpectedLocalStiffnessMatrix = localStiffnessMatrix,
        ExpectedGlobalStiffnessMatrix = localStiffnessMatrix,
        ExpectedLocalFixedEndForces = localfixedEndForces,
        ExpectedGlobalFixedEndForces = localfixedEndForces,
        ExpectedLocalEndDisplacements = localEndDisplacements,
        ExpectedGlobalEndDisplacements = localEndDisplacements,
        ExpectedLocalEndForces = localEndForces,
        ExpectedGlobalEndForces = localEndForces,
      };
    }

    public static Element1DFixture GetElement2Fixture()
    {
      #region ElementDefinition
      var baseLine = new Line(new double[] { 0, -20 * 12, 0 }, new double[] { 0, 0, 0 });
      var section = new SectionProfile()
      {
        A = 32.9,
        Iz = 716,
        Iy = 236,
        J = 15.1
      };
      var material = new Material()
      {
        E = 29000,
        G = 11500
      };

      var element = new Element1D(baseLine, section, material, ElementType.Beam);
      element.ProfileRotation = Math.PI / 2;
      #endregion

      #region ResultsDefinition
      var rotationMatrix = DenseMatrix.OfArray(new double[,]
      {
        { 0, 1, 0 },
        { 0, 0, 1 },
        { 1, 0, 0 }
      });

      var transformationMatrix = DenseMatrix.OfArray(new double[,]
      {
        { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 }
      });

      var globalStiffnessMatrix = DenseMatrix.OfArray(new double[,] {
        { 5.941,        0,       0,          0,       0,   -712.92,   -5.941,        0,        0,         0,       0,   -712.92 },
        {      0,  3975.4,       0,          0,       0,         0,        0,  -3975.4,        0,         0,       0,   0 },
        {      0,       0,  18.024,     2162.9,       0,         0,        0,        0,  -18.024,    2162.9,       0,   0 },
        {      0,       0,  2162.9,     346067,       0,         0,        0,        0,  -2162.9,    173033,       0,   0 },
        {      0,       0,       0,          0,  723.54,         0,        0,        0,        0,         0, -723.54,   0 },
        {-712.92,       0,       0,          0,       0,    114067,   712.92,        0,        0,         0,       0,   57033 },
        { -5.941,       0,       0,          0,       0,    712.92,    5.941,        0,        0,         0,       0,   712.92 },
        {      0, -3975.4,       0,          0,       0,         0,        0,   3975.4,        0,         0,       0,   0 },
        {      0,       0, -18.024,    -2162.9,       0,         0,        0,        0,   18.024,   -2162.9,       0,   0 },
        {      0,       0,  2162.9,     173033,       0,         0,        0,        0,  -2162.9,    346067,       0,   0 },
        {      0,       0,       0,          0, -723.54,         0,        0,        0,        0,         0,  723.54,   0 },
        {-712.92,       0,       0,          0,       0,     57033,   712.92,        0,        0,         0,       0,   114067 },
      });

      var localfixedEndForces = Vector<double>.Build.Dense(new double[]
      {
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0
      });

      var localEndDisplacements = Vector<double>.Build.Dense(new double[]
      {
        0,
        0,
        0,
        0,
        0,
        0,
        -2.7965,
        -1.812,
        -1.3522,
        1.0569,
        6.4986,
        -3.0021
      }) * Math.Pow(10, -3);

      var globalEndDisplacements = Vector<double>.Build.Dense(new double[]
      {
        0,
        0,
        0,
        0,
        0,
        0,
        -1.3522,
        -2.7965,
        -1.812,
        -3.0021,
        1.0569,
        6.4986
      }) * Math.Pow(10, -3);

      var localEndForces = Vector<double>.Build.Dense(new double[]
      {
        11.117,
        -6.4607,
        -4.6249,
        -0.76472,
        369.67,
        -515.55,
        -11.117,
        6.4607,
        4.6249,
        0.76472,
        740.31,
        -1035,
      });

      var globalEndForces = Vector<double>.Build.Dense(new double[]
      {
        -4.6249,
        11.117,
        -6.4607,
        -515.55,
        -0.76472,
        369.67,
        4.6249,
        -11.117,
        6.4607,
        -1,035,
        0.76472,
        740.31,
      });
      #endregion

      return new Element1DFixture(element)
      {
        ExpectedRotationMatrix = rotationMatrix,
        ExpectedTransformationMatrix = transformationMatrix,
        //ExpectedLocalStiffnessMatrix = localStiffnessMatrix,
        ExpectedGlobalStiffnessMatrix = globalStiffnessMatrix,
        ExpectedLocalFixedEndForces = localfixedEndForces,
        ExpectedGlobalFixedEndForces = localfixedEndForces,
        ExpectedLocalEndDisplacements = localEndDisplacements,
        ExpectedGlobalEndDisplacements = localEndDisplacements,
        ExpectedLocalEndForces = localEndForces,
        ExpectedGlobalEndForces = localEndForces,
      };
    }

    public static Element1DFixture GetElement3Fixture()
    {
      #region ElementDefinition
      var baseLine = new Line(new double[] { 0, 0, -20 * 12 }, new double[] { 0, 0, 0 });
      var section = new SectionProfile()
      {
        A = 32.9,
        Iz = 716,
        Iy = 236,
        J = 15.1
      };
      var material = new Material()
      {
        E = 29000,
        G = 11500
      };
      var element = new Element1D(baseLine, section, material, ElementType.Beam);
      element.ProfileRotation = 30 * Math.PI / 180;
      #endregion

      #region ResultsDefinition
      var rotationMatrix = DenseMatrix.OfArray(new double[,]
      {
        {   0,     0,      1 },
        { -.5,     .86603, 0 },
        { -.86603, -.5,      0 }
      });

      var transformationMatrix = DenseMatrix.OfArray(new double[,]
      {
        {0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {-0.5, 0.86603, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {-0.86603, -0.5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, -0.5, 0.86603, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, -0.86603, -0.5, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, -0.5, 0.86603, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, -0.86603, -0.5, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, -0.5, 0.86603, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, -0.86603, -0.5, 0}
      });

      var globalStiffnessMatrix = DenseMatrix.OfArray(new double[,] {
        {8.9618,    -5.2322,    0,          627.87,     1075.4,     0,          -8.9618,    5.2322,     0,          627.87,     1075.4,     0},
        {-5.2322,   15.003,     0,          -1800.4,    -627.87,    0,          5.2322,     -15.003,    0,          -1800.4,   -627.87,     0},
        {0,         0,          3975.4,     0,          0,          0,          0,          0,          -3975.4,    0,          0,          0},
        {627.87,    -1800.4,    0,          288067,     100459,     0,          -627.87,    1800.4,     0,          144033,     50229,      0},
        {1075.4,    -627.87,    0,          100459,     172067,     0,          -1075.4,    627.87,     0,          50229,      86033,      0},
        {0,         0,          0,          0,          0,          723.54,     0,          0,          0,          0,          0,          -723.54},
        {-8.9618,   5.2322,     0,          -627.87,    -1075.4,    0,          8.9618,     -5.2322,    0,          -627.87,    -1075.4,    0},
        {5.2322,    -15.003,    0,          1800.4,     627.87,     0,          -5.2322,    15.003,     0,          1800.4,     627.87,     0},
        {0,         0,          -3975.4,    0,          0,          0,          0,          0,          3975.4,     0,          0,          0},
        {627.87,    -1800.4,    0,          144033,     50229,      0,          -627.87,    1800.4,     0,          288067,     100459,     0},
        {1075.4,    -627.87,    0,          50229,      86033,      0,          -1075.4,    627.87,     0,          100459,     172067,     0},
        {0,         0,          0,          0,          0,          -723.54,    0,          0,          0,          0,          0,          723.54}
      });

      var localfixedEndForces = Vector<double>.Build.Dense(new double[]
      {
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0
      });

      var localEndDisplacements = Vector<double>.Build.Dense(new double[]
      {
        0,
        0,
        0,
        0,
        0,
        0,
        -1.812,
        -1.7457,
        2.5693,
        6.4986,
        2.4164,
        2.0714
      }) * Math.Pow(10, -3);

      var globalEndDisplacements = Vector<double>.Build.Dense(new double[]
      {
        0,
        0,
        0,
        0,
        0,
        0,
        -1.3522,
        -2.7965,
        -1.812,
        -3.0021,
        1.0569,
        6.4986
      }) * Math.Pow(10, -3);

      var localEndForces = Vector<double>.Build.Dense(new double[]
      {
        7.2034,
        4.5118,
        -1.7379,
        -4.702,
        139.65,
        362.21,
        -7.2034,
        -4.5118,
        1.7379,
        4.702,
        277.46,
        720.63,
      });

      var globalEndForces = Vector<double>.Build.Dense(new double[]
      {
        -0.75082,
        4.7763,
        7.2034,
        -383.5,
        -60.166,
        -4.702,
        0.75082,
        -4.7763,
        -7.2034,
        -762.82,
        -120.03,
        4.702,
      });
      #endregion

      return new Element1DFixture(element)
      {
        ExpectedRotationMatrix = rotationMatrix,
        ExpectedTransformationMatrix = transformationMatrix,
        //ExpectedLocalStiffnessMatrix = localStiffnessMatrix,
        ExpectedGlobalStiffnessMatrix = globalStiffnessMatrix,
        ExpectedLocalFixedEndForces = localfixedEndForces,
        ExpectedGlobalFixedEndForces = localfixedEndForces,
        ExpectedLocalEndDisplacements = localEndDisplacements,
        ExpectedGlobalEndDisplacements = localEndDisplacements,
        ExpectedLocalEndForces = localEndForces,
        ExpectedGlobalEndForces = localEndForces,
      };
    }
    #endregion
  }
}
