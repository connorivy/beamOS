using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalElement1Ds;
using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalModels;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalModelAggregate;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalModelAggregate.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalNodeAggregate;
using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.SolvedProblems.MatrixAnalysisOfStructures2ndEd;

internal class Example3_8 : SolvedProblem
{
    public Example3_8()
    {
        this.AnalyticalNodes = AnalyticalNodesStatic;
        this.Element1DFixtures = AnalyticalElementsStatic;
        this.AnalyticalModelFixture = AnalyticalModelFixtureStatic;
    }

    static Example3_8()
    {
        Steel29000 = new(
            new Pressure(29000, PressureUnit.KilopoundForcePerSquareInch),
            new Pressure(1, PressureUnit.KilopoundForcePerSquareInch)
        );
        Area8In = new(
            new Area(8, AreaUnit.SquareInch),
            new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.InchToTheFourth),
            new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.InchToTheFourth),
            new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.InchToTheFourth)
        );
        Area6In = new(
            new Area(6, AreaUnit.SquareInch),
            new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.InchToTheFourth),
            new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.InchToTheFourth),
            new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.InchToTheFourth)
        );

        AnalyticalNode1 ??= GetAnalyticalNode1();
        AnalyticalNode2 = GetAnalyticalNode2();
        AnalyticalNode3 = GetAnalyticalNode3();
        AnalyticalNode4 = GetAnalyticalNode4();
        AnalyticalNodesStatic =
        [
            AnalyticalNode1,
            AnalyticalNode2,
            AnalyticalNode3,
            AnalyticalNode4
        ];

        Element1 = GetElement1Fixture();
        Element2 = GetElement2Fixture();
        Element3 = GetElement3Fixture();
        AnalyticalElementsStatic =  [Element1, Element2, Element3];

        AnalyticalModelFixtureStatic = GetAnalyticalModelFixture(
            AnalyticalNodesStatic,
            AnalyticalElementsStatic.Select(f => f.Element)
        );
    }

    public static DsmNode AnalyticalNode1 { get; }
    public static DsmNode AnalyticalNode2 { get; }
    public static DsmNode AnalyticalNode3 { get; }
    public static DsmNode AnalyticalNode4 { get; }
    public static AnalyticalElement1DFixture Element1 { get; }
    public static AnalyticalElement1DFixture Element2 { get; }
    public static AnalyticalElement1DFixture Element3 { get; }
    public static Material Steel29000 { get; }
    public static SectionProfile Area8In { get; }
    public static SectionProfile Area6In { get; }
    public static List<DsmNode> AnalyticalNodesStatic { get; set; }
    public static List<AnalyticalElement1DFixture> AnalyticalElementsStatic { get; set; }
    public static AnalyticalModelFixture AnalyticalModelFixtureStatic { get; set; }

    public static AnalyticalModelFixture GetAnalyticalModelFixture(
        IEnumerable<DsmNode> nodes,
        IEnumerable<AnalyticalElement1D> els
    )
    {
        var model = AnalyticalModel.RunAnalysis(UnitSettings.K_IN, els, nodes);

        var supportReactionVector = DenseVector.OfArray(
            [0, 0, 0, -10.064, -13.419, 0, 0, 0, 0, 126.83, 0, 0, 0, -139.94, 186.58, 0, 0, 0]
        );

        var supportDisplacementVector = DenseVector.OfArray([.21552, -.13995, 0, 0, 0, 0]);

        return new AnalyticalModelFixture(model)
        {
            ExpectedDisplacementVector = supportDisplacementVector,
            ExpectedReactionVector = supportReactionVector,
        };
    }

    private static DsmNode GetAnalyticalNode1()
    {
        Restraint free2D = new(true, true, false, false, false, true);
        List<PointLoad> pointLoads =
        [
            new PointLoad(
                new Force(150, ForceUnit.KilopoundForce),
                DenseVector.OfArray([1, 0, 0])
            ),
            new PointLoad(
                new Force(300, ForceUnit.KilopoundForce),
                DenseVector.OfArray([0, -1, 0])
            )
        ];
        return new(12, 16, 0, LengthUnit.Foot, free2D, pointLoads, id: new(Constants.Guid1));
    }

    private static DsmNode GetAnalyticalNode2()
    {
        Restraint pinned2d = new(false, false, false, false, false, true);
        return new(0, 0, 0, LengthUnit.Foot, pinned2d, id: new(Constants.Guid2));
    }

    private static DsmNode GetAnalyticalNode3()
    {
        Restraint pinned2d = new(false, false, false, false, false, true);
        return new(12, 0, 0, LengthUnit.Foot, pinned2d, id: new(Constants.Guid3));
    }

    private static DsmNode GetAnalyticalNode4()
    {
        Restraint pinned2d = new(false, false, false, false, false, true);
        return new(24, 0, 0, LengthUnit.Foot, pinned2d, id: new(Constants.Guid4));
    }

    private static AnalyticalElement1DFixture GetElement1Fixture()
    {
        AnalyticalElement1D el =
            new(Angle.Zero, AnalyticalNode2, AnalyticalNode1, Steel29000, Area8In);

        var localStiffnessMatrix = DenseMatrix.OfArray(
            new double[,]
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
            }
        );

        return new AnalyticalElement1DFixture(el, UnitSettings.K_IN)
        {
            //ExpectedRotationMatrix = rotationMatrix,
            //ExpectedTransformationMatrix = transformationMatrix,
            //ExpectedLocalStiffnessMatrix = localStiffnessMatrix,
            //ExpectedGlobalStiffnessMatrix = localStiffnessMatrix,
            //ExpectedLocalFixedEndForces = localFixedEndForces,
            //ExpectedGlobalFixedEndForces = localFixedEndForces,
            //ExpectedLocalEndDisplacements = localEndDisplacements,
            //ExpectedGlobalEndDisplacements = localEndDisplacements,
            //ExpectedLocalEndForces = localEndForces,
            //ExpectedGlobalEndForces = localEndForces,
        };
    }

    private static AnalyticalElement1DFixture GetElement2Fixture()
    {
        AnalyticalElement1D el =
            new(Angle.Zero, AnalyticalNode3, AnalyticalNode1, Steel29000, Area6In);

        return new AnalyticalElement1DFixture(el, UnitSettings.K_IN);
    }

    private static AnalyticalElement1DFixture GetElement3Fixture()
    {
        AnalyticalElement1D el =
            new(Angle.Zero, AnalyticalNode4, AnalyticalNode1, Steel29000, Area8In);

        return new AnalyticalElement1DFixture(el, UnitSettings.K_IN);
    }
}
