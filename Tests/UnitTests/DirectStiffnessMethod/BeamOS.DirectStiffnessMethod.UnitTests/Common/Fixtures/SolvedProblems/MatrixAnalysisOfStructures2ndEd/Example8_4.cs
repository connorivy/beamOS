using BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalElement1Ds;
using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalModels;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.SolvedProblems.MatrixAnalysisOfStructures2ndEd;

internal partial class Example8_4 : SolvedProblem
{
    public Example8_4()
    {
        this.Element1DFixtures.Add(Element1);
        this.Element1DFixtures.Add(Element2);
        this.Element1DFixtures.Add(Element3);

        this.AnalyticalModelFixture = ModelFixture;
    }

    static Example8_4()
    {
        AnalyticalNode1 = GetAnalyticalNode1();
        AnalyticalNode2 = GetAnalyticalNode2();
        AnalyticalNode3 = GetAnalyticalNode3();
        AnalyticalNode4 = GetAnalyticalNode4();

        Element1 = GetElement1Fixture();
        Element2 = GetElement2Fixture();
        Element3 = GetElement3Fixture();

        ModelFixture = GetAnalyticalModel();
    }

    #region AnalyticalModelFixtureDefinition
    public static AnalyticalModelFixture ModelFixture { get; }

    public static AnalyticalModelFixture GetAnalyticalModel()
    {
        #region ModelDefinition
        //var am = new AnalyticalModel(Element1.Element.BaseCurve.EndNode0.Position);

        //am.AddElement1D(Element1.Element);
        //am.AddElement1D(Element2.Element);
        //am.AddElement1D(Element3.Element);

        #endregion

        #region ResultsDefinition

        #endregion

        return new AnalyticalModelFixture();
    }
    #endregion

    #region Element1DFixtureDefinitions
    public static AnalyticalElement1DFixture Element1 { get; }
    public static AnalyticalElement1DFixture Element2 { get; }
    public static AnalyticalElement1DFixture Element3 { get; }
    public static SectionProfile Profile33in2 => new(
        new Area(32.9, AreaUnit.SquareInch),
        strongAxisMomentOfInertia: new AreaMomentOfInertia(716, AreaMomentOfInertiaUnit.InchToTheFourth),
        weakAxisMomentOfInertia: new AreaMomentOfInertia(236, AreaMomentOfInertiaUnit.InchToTheFourth),
        polarMomentOfInertia: new AreaMomentOfInertia(15.1, AreaMomentOfInertiaUnit.InchToTheFourth)
    );
    public static Material Steel29000ksi => new(
        modulusOfElasticity: new Pressure(29000, PressureUnit.KilopoundForcePerSquareInch),
        modulusOfRigidity: new Pressure(11500, PressureUnit.KilopoundForcePerSquareInch)
    );

    private static AnalyticalElement1DFixture GetElement1Fixture()
    {
        AnalyticalElement1D element = new(
            Angle.Zero,
            AnalyticalNode2,
            AnalyticalNode1,
            Steel29000ksi,
            Profile33in2,
            new(Constants.Guid1));

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
            { 0,       0,        -712.92,  0,       114066.7, 0,        0,        0,        712.92,   0,        57033.3,  0       },
            { 0,       2162.9,   0,        0,       0,        346066.7, 0,        -2162.9,  0,        0,        0,        173033.3  },
            { -3975.4, 0,        0,        0,       0,        0,        3975.4,   0,        0,        0,        0,        0       },
            { 0,       -18.024,  0,        0,       0,        -2162.9,  0,        18.024,   0,        0,        0,        -2162.9 },
            { 0,       0,        -5.941,   0,       712.92,   0,        0,        0,        5.941,    0,        712.92,   0       },
            { 0,       0,        0,        -723.54, 0,        0,        0,        0,        0,        723.54,   0,        0       },
            { 0,       0,        -712.92,  0,       57033.3,  0,        0,        0,        712.92,   0,        114066.7, 0       },
            { 0,       2162.9,   0,        0,       0,        173033.3, 0,        -2162.9,  0,        0,        0,        346066.7  }
        });

        var localFixedEndForces = Vector<double>.Build.Dense(
        [
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
        ]);

        var localEndDisplacements = Vector<double>.Build.Dense(
        [
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
        ]) * Math.Pow(10, -3);

        var localEndForces = Vector<double>.Build.Dense(
        [
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
        ]);
        #endregion

        return new AnalyticalElement1DFixture(element, UnitSettings.K_IN)
        {
            ExpectedRotationMatrix = rotationMatrix,
            ExpectedTransformationMatrix = transformationMatrix,
            ExpectedLocalStiffnessMatrix = localStiffnessMatrix,
            ExpectedGlobalStiffnessMatrix = localStiffnessMatrix,
            ExpectedLocalFixedEndForces = localFixedEndForces,
            ExpectedGlobalFixedEndForces = localFixedEndForces,
            ExpectedLocalEndDisplacements = localEndDisplacements,
            ExpectedGlobalEndDisplacements = localEndDisplacements,
            ExpectedLocalEndForces = localEndForces,
            ExpectedGlobalEndForces = localEndForces,
        };
    }

    public static AnalyticalElement1DFixture GetElement2Fixture()
    {
        AnalyticalElement1D element = new(
            new Angle(Math.PI / 2, AngleUnit.Radian),
            AnalyticalNode3,
            AnalyticalNode1,
            Steel29000ksi,
            Profile33in2,
            new(Constants.Guid2));

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
            {      0,       0,  2162.9,   346066.7,       0,         0,        0,        0,  -2162.9,  173033.3,       0,   0 },
            {      0,       0,       0,          0,  723.54,         0,        0,        0,        0,         0, -723.54,   0 },
            {-712.92,       0,       0,          0,       0,  114066.7,   712.92,        0,        0,         0,       0,   57033.3 },
            { -5.941,       0,       0,          0,       0,    712.92,    5.941,        0,        0,         0,       0,   712.92 },
            {      0, -3975.4,       0,          0,       0,         0,        0,   3975.4,        0,         0,       0,   0 },
            {      0,       0, -18.024,    -2162.9,       0,         0,        0,        0,   18.024,   -2162.9,       0,   0 },
            {      0,       0,  2162.9,   173033.3,       0,         0,        0,        0,  -2162.9,  346066.7,       0,   0 },
            {      0,       0,       0,          0, -723.54,         0,        0,        0,        0,         0,  723.54,   0 },
            {-712.92,       0,       0,          0,       0,   57033.3,   712.92,        0,        0,         0,       0,   114066.7 },
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

        return new AnalyticalElement1DFixture(element, UnitSettings.K_IN)
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

    public static AnalyticalElement1DFixture GetElement3Fixture()
    {
        AnalyticalElement1D element = new(
            new Angle(30, AngleUnit.Degree),
            AnalyticalNode4,
            AnalyticalNode1,
            Steel29000ksi,
            Profile33in2,
            new(Constants.Guid3));

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
            {627.87,    -1800.4,    0,          288066.7,   100458.9,   0,          -627.87,    1800.4,     0,          144033.3,   50229.5,    0},
            {1075.4,    -627.87,    0,          100458.9,   172066.7,   0,          -1075.4,    627.87,     0,          50229.5,    86033.3,    0},
            {0,         0,          0,          0,          0,          723.54,     0,          0,          0,          0,          0,          -723.54},
            {-8.9618,   5.2322,     0,          -627.87,    -1075.4,    0,          8.9618,     -5.2322,    0,          -627.87,    -1075.4,    0},
            {5.2322,    -15.003,    0,          1800.4,     627.87,     0,          -5.2322,    15.003,     0,          1800.4,     627.87,     0},
            {0,         0,          -3975.4,    0,          0,          0,          0,          0,          3975.4,     0,          0,          0},
            {627.87,    -1800.4,    0,          144033.3,   50229.5,    0,          -627.87,    1800.4,     0,          288066.7,   100458.9,   0},
            {1075.4,    -627.87,    0,          50229.5,    86033.3,    0,          -1075.4,    627.87,     0,          100458.9,   172066.7,   0},
            {0,         0,          0,          0,          0,          -723.54,    0,          0,          0,          0,          0,          723.54}
        });

        var localFixedEndForces = Vector<double>.Build.Dense(new double[]
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

        return new AnalyticalElement1DFixture(element, UnitSettings.K_IN)
        {
            ExpectedRotationMatrix = rotationMatrix,
            ExpectedTransformationMatrix = transformationMatrix,
            //ExpectedLocalStiffnessMatrix = localStiffnessMatrix,
            ExpectedGlobalStiffnessMatrix = globalStiffnessMatrix,
            ExpectedLocalFixedEndForces = localFixedEndForces,
            ExpectedGlobalFixedEndForces = localFixedEndForces,
            ExpectedLocalEndDisplacements = localEndDisplacements,
            ExpectedGlobalEndDisplacements = localEndDisplacements,
            ExpectedLocalEndForces = localEndForces,
            ExpectedGlobalEndForces = localEndForces,
        };
    }
    #endregion
}
