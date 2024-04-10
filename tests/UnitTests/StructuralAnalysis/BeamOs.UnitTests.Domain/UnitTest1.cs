using AngouriMath;
using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.Diagrams;
using BeamOs.Domain.Diagrams.Common.ValueObjects;
using BeamOs.Domain.Diagrams.Common.ValueObjects.DiagramBuilder;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;
using static AngouriMath.MathS;

namespace BeamOs.UnitTests.Domain;

public class UnitTest1
{
    //[Fact]
    //public void Test1()
    //{
    //    ModelSettings modelSettings = new(UnitSettings.K_IN);
    //    Model model = new("test model", "", modelSettings);
    //    Node startNode = new(model.Id, new(0, 0, 0, LengthUnit.Inch));
    //    Node endNode = new(model.Id, new(120, 0, 0, LengthUnit.Inch));
    //    Element1D element1D =
    //        new(model.Id, startNode.Id, endNode.Id, new MaterialId(), new SectionProfileId());

    //    ImmutablePointLoad pl1 =
    //        new(new Force(10, ForceUnit.KilopoundForce), DenseVector.OfArray([0, -1.0, 0]));
    //    element1D.AddPointLoad(new Ratio(50, RatioUnit.Percent), pl1);

    //    ShearForceDiagramAggregate shearForceDiagram =
    //        new(
    //            new()
    //            {
    //                {
    //                    0.0,
    //                    new ImmutablePointLoad(
    //                        new Force(5, ForceUnit.KilopoundForce),
    //                        DenseVector.OfArray([0, 1.0, 0])
    //                    )
    //                },
    //                {
    //                    1.0,
    //                    new ImmutablePointLoad(
    //                        new Force(5, ForceUnit.KilopoundForce),
    //                        DenseVector.OfArray([0, 1.0, 0])
    //                    )
    //                },
    //                { 0.5, pl1 },
    //            }
    //        );

    //    shearForceDiagram.Build(CoordinateSystemDirection3D.AlongY, UnitSettings.K_IN);
    //    shearForceDiagram.Integrate(0);
    //}

    //[Fact]
    //public void Test3()
    //{
    //    ModelSettings modelSettings = new(UnitSettings.K_IN);
    //    Model model = new("test model", "", modelSettings);
    //    Node startNode = new(model.Id, new(0, 0, 0, LengthUnit.Inch));
    //    Node endNode = new(model.Id, new(120, 0, 0, LengthUnit.Inch));
    //    Element1D element1D =
    //        new(model.Id, startNode.Id, endNode.Id, new MaterialId(), new SectionProfileId());

    //    ImmutablePointLoad pl1 =
    //        new(new Force(10, ForceUnit.KilopoundForce), DenseVector.OfArray([0, -1.0, 0]));
    //    element1D.AddPointLoad(new Ratio(50, RatioUnit.Percent), pl1);

    //    ShearForceDiagramAggregate shearForceDiagram =
    //        new(
    //            new()
    //            {
    //                {
    //                    0.0,
    //                    new ImmutablePointLoad(
    //                        new Force(12, ForceUnit.KilopoundForce),
    //                        DenseVector.OfArray([0, 1.0, 0])
    //                    )
    //                },
    //                {
    //                    1.0,
    //                    new ImmutablePointLoad(
    //                        new Force(9, ForceUnit.KilopoundForce),
    //                        DenseVector.OfArray([0, 1.0, 0])
    //                    )
    //                },
    //                {
    //                    0.333,
    //                    new ImmutablePointLoad(
    //                        new Force(15, ForceUnit.KilopoundForce),
    //                        DenseVector.OfArray([0, -1.0, 0])
    //                    )
    //                },
    //                {
    //                    0.667,
    //                    new ImmutablePointLoad(
    //                        new Force(6, ForceUnit.KilopoundForce),
    //                        DenseVector.OfArray([0, -1.0, 0])
    //                    )
    //                },
    //            }
    //        );

    //    shearForceDiagram.Build(CoordinateSystemDirection3D.AlongY, UnitSettings.K_IN);
    //    shearForceDiagram.Integrate(0);
    //}

    //[Fact]
    //public void Test2()
    //{
    //    Diagram d = Diagram.Build(

    //        [
    //            new(new Ratio(0.0, RatioUnit.DecimalFraction), new Polynomial(55.97)),
    //            new(new Ratio(1.0, RatioUnit.DecimalFraction), new Polynomial(10.03)),
    //            new(new Ratio(0.5, RatioUnit.DecimalFraction), new Polynomial(-20.0)),
    //            new(new Ratio(.75, RatioUnit.DecimalFraction), new Polynomial(-10.0)),
    //        ],

    //        [
    //            new(new Ratio(0.0, RatioUnit.DecimalFraction), new Ratio(0.5, RatioUnit.DecimalFraction), new Polynomial(-9.0))
    //        ],
    //        new Length(8, LengthUnit.Meter),
    //        LengthUnit.Meter
    //    );

    //    Diagram moment = d.Integrate(

    //        [
    //            new(new Ratio(0, RatioUnit.DecimalFraction), new Polynomial(-91.78)),
    //            new(new Ratio(.5, RatioUnit.DecimalFraction), new Polynomial(-40.0))
    //        ],
    //        []
    //    );
    //}

    //[Fact]
    //public void Test1()
    //{
    //    Diagram d = Diagram.Build(

    //        [
    //            new(new Ratio(0.0, RatioUnit.DecimalFraction), new Polynomial(-6.0)),
    //            new(new Ratio(0.1875, RatioUnit.DecimalFraction), new Polynomial(4.2)),
    //            new(new Ratio(0.8125, RatioUnit.DecimalFraction), new Polynomial(7.8)),
    //        ],

    //        [
    //            new(new Ratio(0.625, RatioUnit.DecimalFraction), new Ratio(1.0, RatioUnit.DecimalFraction), new Polynomial(-1.0))
    //        ],
    //        new Length(16, LengthUnit.Meter),
    //        LengthUnit.Meter
    //    );

    //    Diagram moment = d.Integrate(
    //        [new(new Ratio(0.1875, RatioUnit.DecimalFraction), new Polynomial(36.0)),]
    //    );

    //    var y = moment.Integrate();
    //    var z = y.Integrate();
    //}

    //[Fact]
    //public void Test3()
    //{
    //    Diagram d = Diagram.Build(

    //        [
    //            new(new Ratio(0.0, RatioUnit.DecimalFraction), new Polynomial(27.5)),
    //            new(new Ratio(1.0, RatioUnit.DecimalFraction), new Polynomial(12.5)),
    //            new(new Ratio(0.5, RatioUnit.DecimalFraction), new Polynomial(-40.0)),
    //        ],
    //        [],
    //        new Length(8, LengthUnit.Meter),
    //        LengthUnit.Meter
    //    );

    //    Diagram moment = d.Integrate(
    //        [new(new Ratio(0.0, RatioUnit.DecimalFraction), new Polynomial(-60.05)),]
    //    );

    //    var y = moment.Integrate();
    //    var z = y.Integrate();
    //}

    /// <summary>
    /// https://temple.manifoldapp.org/read/structural-analysis/section/072c774b-392a-4b99-a65d-f2e9d1fddfbd#:~:text=The%20moment%2Darea%20method%20uses,method%2C%20which%20are%20derived%20below.
    /// Example 7.6
    /// </summary>
    [Fact]
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
    }

    /// <summary>
    /// https://temple.manifoldapp.org/read/structural-analysis/section/072c774b-392a-4b99-a65d-f2e9d1fddfbd#:~:text=The%20moment%2Darea%20method%20uses,method%2C%20which%20are%20derived%20below.
    /// Example 7.6
    /// </summary>
    [Fact]
    public void Test6()
    {
        Diagram shearDiagram = new DiagramBuilder(
            new Length(32, LengthUnit.Foot),
            new Length(1, LengthUnit.Inch),
            LengthUnit.Foot
        )
            .AddPointLoads(
                new DiagramPointValue(new Length(0, LengthUnit.Foot), 4),
                new DiagramPointValue(new Length(16, LengthUnit.Foot), -5),
                new DiagramPointValue(new Length(20, LengthUnit.Foot), 45),
                new DiagramPointValue(new Length(32, LengthUnit.Foot), -6)
            )
            .AddDistributedLoads(
                new DiagramDistributedValue(0, 10, LengthUnit.Foot, new Polynomial(-2.0)),
                new DiagramDistributedValue(20, 26, LengthUnit.Foot, new Polynomial(.5 * 20, -.5)),
                new DiagramDistributedValue(26, 32, LengthUnit.Foot, new Polynomial(.5 * -32, .5))
            )
            .Build();

        Diagram momentDiagram = shearDiagram
            .Integrate()
            .ApplyIntegrationBoundaryConditions(
                1,
                new DiagramPointValue(new Length(0, LengthUnit.Foot), 60),
                new DiagramPointValue(new Length(32, LengthUnit.Foot), 0),
                new DiagramPointValue(new Length(10, LengthUnit.Foot), 0)
            )
            .Build();

        Diagram slopeDiagram = momentDiagram.Integrate().Build();

        Diagram deflectionDiagram = slopeDiagram
            .Integrate()
            .ApplyIntegrationBoundaryConditions(
                2,
                new DiagramPointValue(new Length(0, LengthUnit.Foot), 0),
                new DiagramPointValue(new Length(8, LengthUnit.Foot), 0)
            )
            .Build();
    }
}
