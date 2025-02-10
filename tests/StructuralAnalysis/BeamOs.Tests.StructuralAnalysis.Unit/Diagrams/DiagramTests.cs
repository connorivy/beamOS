using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common.ValueObjects.DiagramBuilder;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using FluentAssertions;
using MathNet.Numerics;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Tests.StructuralAnalysis.Unit.Diagrams;

public class DiagramTests
{
    /// <summary>
    /// https://temple.manifoldapp.org/read/structural-analysis/section/072c774b-392a-4b99-a65d-f2e9d1fddfbd#:~:text=The%20moment%2Darea%20method%20uses,method%2C%20which%20are%20derived%20below.
    /// Example 7.6
    /// </summary>
    [Test]
    public void Test5()
    {
        Diagram shearDiagram = new DiagramBuilder(
            new Length(11, LengthUnit.Meter),
            new Length(1, LengthUnit.Centimeter),
            LengthUnit.Meter
        )
            .AddPointLoads(
                new DiagramPointValue(new Length(0, LengthUnit.Meter), 96.25),
                new DiagramPointValue(new Length(8, LengthUnit.Meter), 433.75),
                new DiagramPointValue(new Length(11, LengthUnit.Meter), -50)
            )
            .AddDistributedLoads(
                new DiagramDistributedValue(
                    new Length(4, LengthUnit.Meter),
                    new Length(8, LengthUnit.Meter),
                    new Polynomial(-120.0)
                )
            )
            .Build();

        //shearDiagram.Intervals[0].Polynomial.Coefficients[0].Should().BeApproximately(96.25, 0.001);
        shearDiagram
            .Intervals[1]
            .Polynomial
            .Coefficients[0]
            .Should()
            .BeApproximately(96.25, 0.001);
        //Assert.Equal(shearDiagram.Intervals[0].Polynomial.Coefficients[0], 96.25);

        Diagram momentDiagram = shearDiagram
            .Integrate()
            .AddPointLoads(new DiagramPointValue(new Length(2, LengthUnit.Meter), 40))
            .ApplyIntegrationBoundaryConditions(
                1,
                new DiagramPointValue(new Length(0, LengthUnit.Meter), 0),
                new DiagramPointValue(new Length(11, LengthUnit.Meter), 0)
            )
            .Build();

        Diagram slopeDiagram = momentDiagram.Integrate().Build();

        Diagram deflectionDiagram = slopeDiagram
            .Integrate()
            .ApplyIntegrationBoundaryConditions(
                2,
                new DiagramPointValue(new Length(0, LengthUnit.Meter), 0),
                new DiagramPointValue(new Length(8, LengthUnit.Meter), 0)
            )
            .Build();

        //Assert.Equal(-956.67, slopeDiagram.GetValueAtLocation(Length.Zero).ValueOnRight, 3);
        //Assert.Equal(
        //    -1785,
        //    deflectionDiagram.GetValueAtLocation(new Length(2, LengthUnit.Meter)).ValueOnLeft,
        //    1
        //);
        deflectionDiagram
            .GetValueAtLocation(new Length(2, LengthUnit.Meter))
            .ValueOnLeft
            .Should()
            .BeApproximately(-1785, 1);
    }

    [Test]
    public void Test1()
    {
        //Func<double, double, double> n1 = (x, l) =>
        //    1 - 3 * Math.Pow(x / l, 2) + 2 * Math.Pow(x / l, 3);
        //Func<double, double, double> n2 = (x, l) => x * Math.Pow(1 - x / l, 2);
        //Func<double, double, double> n3 = (x, l) => 3 * Math.Pow(x / l, 2) - 2 * Math.Pow(x / l, 3);
        //Func<double, double, double> n4 = (x, l) => -Math.Pow(x, 2) / l * (1 - x / l);

        //Func<double, double, double[,]> Nx = (x, l) =>
        //    new double[6, 12]
        //    {
        //        { n1(x, l), 0, 0, 0, 0, 0, n2(x, l), 0, 0, 0, 0, 0 },
        //        { 0, n1(x, 1), 0, 0, 0, n2(x, l), 0, n3(x, l), 0, 0, 0, n4(x, l) },
        //        { 0, 0, n1(x, 1), 0, n2(x, l), 0, 0, 0, n3(x, l), 0, n4(x, l), 0 },
        //        { 0, 0, 0, n1(x, 1), 0, 0, 0, 0, 0, n2(x, l), 0, 0 },
        //        { 0, 0, n1(x, 1), 0, n2(x, l), 0, 0, 0, n3(x, l), 0, n4(x, l), 0 },
        //        { 0, n1(x, 1), 0, 0, 0, n2(x, l), 0, n3(x, l), 0, 0, 0, n4(x, l) },
        //    };

        //DsmElement1d dsmElement1D =
        //    new(
        //        new Element1dId(),
        //        Angle.Zero,
        //        new Node(
        //            new ModelId(),
        //            new Point(0, 0, 0, LengthUnit.Meter),
        //            Restraint.PinnedInXyPlane
        //        ),
        //        new Node(
        //            new ModelId(),
        //            new Point(11, 0, 0, LengthUnit.Meter),
        //            new Restraint(true, false, false, false, false, false)
        //        ),
        //        new Material(
        //            new ModelId(),
        //            new(1, PressureUnit.KilonewtonPerSquareMeter),
        //            new(1, PressureUnit.KilonewtonPerSquareMeter)
        //        ),
        //        new SectionProfile(
        //            new ModelId(),
        //            new(1, AreaUnit.SquareMeter),
        //            new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.MeterToTheFourth),
        //            new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.MeterToTheFourth),
        //            new(1, AreaMomentOfInertiaUnit.MeterToTheFourth),
        //            new(1, AreaUnit.SquareMeter),
        //            new(1, AreaUnit.SquareMeter)
        //        )
        //    );

        //dsmElement1D.GetLocalEndDisplacementVector();

        //BeamOsDynamicModelBuilder modelBuilder =
        //    new(
        //        "11585814-9bd1-447c-a5a5-b945c360a1f5",
        //        new(BeamOs.StructuralAnalysis.Contracts.Common.UnitSettingsContract.kN_M)
        //    );
        //modelBuilder.AddNodes(
        //    new CreateNodeRequest()
        //    {
        //        LocationPoint = new(0, 0, 0, LengthUnitContract.Meter),
        //        Restraint = BeamOs.StructuralAnalysis.Contracts.Common.Restraint.PinnedXyPlane,
        //        Id = 1
        //    },
        //    new CreateNodeRequest()
        //    {
        //        LocationPoint = new(11, 0, 0, LengthUnitContract.Meter),
        //        Restraint = new(true, false, false, false, false, false),
        //        Id = 2
        //    }
        //);
        //modelBuilder.AddMaterials(
        //    new CreateMaterialRequest()
        //    {
        //        ModulusOfElasticity = new(1, PressureUnitContract.KilonewtonPerSquareMeter),
        //        ModulusOfRigidity = new(1, PressureUnitContract.KilonewtonPerSquareMeter)
        //    }
        //);
        //modelBuilder.AddSectionProfiles(
        //    new CreateSectionProfileRequest()
        //    {
        //        Area = new(1, AreaUnitContract.SquareMeter),
        //        StrongAxisMomentOfInertia = new(
        //            1,
        //            AreaMomentOfInertiaUnitContract.MeterToTheFourth
        //        ),
        //        WeakAxisMomentOfInertia = new(1, AreaMomentOfInertiaUnitContract.MeterToTheFourth),
        //        PolarMomentOfInertia = new(1, AreaMomentOfInertiaUnitContract.MeterToTheFourth),
        //        StrongAxisShearArea = new(1, AreaUnitContract.SquareMeter),
        //        WeakAxisShearArea = new(1, AreaUnitContract.SquareMeter)
        //    }
        //);
        //modelBuilder.AddPointLoads(
        //    new CreatePointLoadRequest()
        //    {
        //        NodeId = 1,
        //        Force = new(96.25, ForceUnitContract.Kilonewton)
        //    },
        //    new CreatePointLoadRequest()
        //    {
        //        NodeId = 2,
        //        Force = new(-50, ForceUnitContract.Kilonewton)
        //    }
        //);
        //var localEndForces = dsmElement1D.GetLocalMemberEndForcesVector()

        Diagram shearDiagram = new DiagramBuilder(
            new Length(11, LengthUnit.Meter),
            new Length(1, LengthUnit.Centimeter),
            LengthUnit.Meter
        )
            .AddPointLoads(
                new DiagramPointValue(new Length(0, LengthUnit.Meter), 96.25),
                new DiagramPointValue(new Length(8, LengthUnit.Meter), 433.75),
                new DiagramPointValue(new Length(11, LengthUnit.Meter), -50)
            )
            .AddDistributedLoads(
                new DiagramDistributedValue(
                    new Length(4, LengthUnit.Meter),
                    new Length(8, LengthUnit.Meter),
                    new Polynomial(-120.0)
                )
            )
            .Build();

        shearDiagram.Intervals[0].Polynomial.Coefficients[0].Should().BeApproximately(96.25, 0.001);
        //Assert.Equal(shearDiagram.Intervals[0].Polynomial.Coefficients[0], 96.25);

        Diagram momentDiagram = shearDiagram
            .Integrate()
            .AddPointLoads(new DiagramPointValue(new Length(2, LengthUnit.Meter), 40))
            .ApplyIntegrationBoundaryConditions(
                1,
                new DiagramPointValue(new Length(0, LengthUnit.Meter), 0),
                new DiagramPointValue(new Length(11, LengthUnit.Meter), 0)
            )
            .Build();

        Diagram slopeDiagram = momentDiagram.Integrate().Build();

        Diagram deflectionDiagram = slopeDiagram
            .Integrate()
            .ApplyIntegrationBoundaryConditions(
                2,
                new DiagramPointValue(new Length(0, LengthUnit.Meter), 0),
                new DiagramPointValue(new Length(8, LengthUnit.Meter), 0)
            )
            .Build();

        //Assert.Equal(-956.67, slopeDiagram.GetValueAtLocation(Length.Zero).ValueOnRight, 3);
        //Assert.Equal(
        //    -1785,
        //    deflectionDiagram.GetValueAtLocation(new Length(2, LengthUnit.Meter)).ValueOnLeft,
        //    1
        //);
        deflectionDiagram
            .GetValueAtLocation(new Length(2, LengthUnit.Meter))
            .ValueOnLeft
            .Should()
            .BeApproximately(-1785, 1);
    }
}
