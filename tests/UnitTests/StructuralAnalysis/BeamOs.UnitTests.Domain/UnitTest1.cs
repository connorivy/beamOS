using AngouriMath;
using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.Diagrams;
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
    [Fact]
    public void Test1()
    {
        ModelSettings modelSettings = new(UnitSettings.K_IN);
        Model model = new("test model", "", modelSettings);
        Node startNode = new(model.Id, new(0, 0, 0, LengthUnit.Inch));
        Node endNode = new(model.Id, new(120, 0, 0, LengthUnit.Inch));
        Element1D element1D =
            new(model.Id, startNode.Id, endNode.Id, new MaterialId(), new SectionProfileId());

        ImmutablePointLoad pl1 =
            new(new Force(10, ForceUnit.KilopoundForce), DenseVector.OfArray([0, -1.0, 0]));
        element1D.AddPointLoad(new Ratio(50, RatioUnit.Percent), pl1);

        ShearForceDiagramAggregate shearForceDiagram =
            new(
                new()
                {
                    {
                        0.0,
                        new ImmutablePointLoad(
                            new Force(5, ForceUnit.KilopoundForce),
                            DenseVector.OfArray([0, 1.0, 0])
                        )
                    },
                    {
                        1.0,
                        new ImmutablePointLoad(
                            new Force(5, ForceUnit.KilopoundForce),
                            DenseVector.OfArray([0, 1.0, 0])
                        )
                    },
                    { 0.5, pl1 },
                }
            );

        shearForceDiagram.Build(CoordinateSystemDirection3D.AlongY, UnitSettings.K_IN);
        shearForceDiagram.Integrate(0);
    }

    [Fact]
    public void Test3()
    {
        ModelSettings modelSettings = new(UnitSettings.K_IN);
        Model model = new("test model", "", modelSettings);
        Node startNode = new(model.Id, new(0, 0, 0, LengthUnit.Inch));
        Node endNode = new(model.Id, new(120, 0, 0, LengthUnit.Inch));
        Element1D element1D =
            new(model.Id, startNode.Id, endNode.Id, new MaterialId(), new SectionProfileId());

        ImmutablePointLoad pl1 =
            new(new Force(10, ForceUnit.KilopoundForce), DenseVector.OfArray([0, -1.0, 0]));
        element1D.AddPointLoad(new Ratio(50, RatioUnit.Percent), pl1);

        ShearForceDiagramAggregate shearForceDiagram =
            new(
                new()
                {
                    {
                        0.0,
                        new ImmutablePointLoad(
                            new Force(12, ForceUnit.KilopoundForce),
                            DenseVector.OfArray([0, 1.0, 0])
                        )
                    },
                    {
                        1.0,
                        new ImmutablePointLoad(
                            new Force(9, ForceUnit.KilopoundForce),
                            DenseVector.OfArray([0, 1.0, 0])
                        )
                    },
                    {
                        0.333,
                        new ImmutablePointLoad(
                            new Force(15, ForceUnit.KilopoundForce),
                            DenseVector.OfArray([0, -1.0, 0])
                        )
                    },
                    {
                        0.667,
                        new ImmutablePointLoad(
                            new Force(6, ForceUnit.KilopoundForce),
                            DenseVector.OfArray([0, -1.0, 0])
                        )
                    },
                }
            );

        shearForceDiagram.Build(CoordinateSystemDirection3D.AlongY, UnitSettings.K_IN);
        shearForceDiagram.Integrate(0);
    }

    [Fact]
    public void Test2()
    {
        Diagram d = new();
        d.Build(

            [
                new(0.0, new Polynomial(55.97)),
                new(1.0, new Polynomial(10.03)),
                new(.5, new Polynomial(-20.0)),
                new(.75, new Polynomial(-10.0)),
            ],
            [new(0, .5, new Polynomial(-9.0))],
            new Length(8, LengthUnit.Meter)
        );
    }
}
