using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;

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

        PointLoadData pl1 =
            new(new Force(10, ForceUnit.KilopoundForce), DenseVector.OfArray([0, -1.0, 0]));
        //element1D.AddPointLoad(new Ratio(50, RatioUnit.Percent), pl1);

        //ShearForceDiagram shearForceDiagram =
        //    new(
        //        new()
        //        {
        //            {
        //                0.0,
        //                new ImmutablePointLoad(
        //                    new Force(5, ForceUnit.KilopoundForce),
        //                    DenseVector.OfArray([0, 1.0, 0])
        //                )
        //            },
        //            {
        //                1.0,
        //                new ImmutablePointLoad(
        //                    new Force(5, ForceUnit.KilopoundForce),
        //                    DenseVector.OfArray([0, 1.0, 0])
        //                )
        //            },
        //            { 0.5, pl1 },
        //        }
        //    );
        var shearForceDiagram = ShearForceDiagram.Create(
            new Length(120, LengthUnit.Inch),
            DenseVector.OfArray([0, 1.0, 0]),
            UnitSettings.K_IN,

            [
                (new Length(), new PointLoadData(new Force(5, ForceUnit.KilopoundForce), DenseVector.OfArray([0, 1.0, 0]))),
                (new Length(120, LengthUnit.Inch), new PointLoadData(new Force(5, ForceUnit.KilopoundForce), DenseVector.OfArray([0, 1.0, 0]))),
                (new Length(60, LengthUnit.Inch), new PointLoadData(new Force(10, ForceUnit.KilopoundForce), DenseVector.OfArray([0, -1.0, 0]))),
            ]
        );

        //shearForceDiagram.Build(CoordinateSystemDirection3D.AlongY, UnitSettings.K_IN);
        //shearForceDiagram.Integrate(0);
    }
}
