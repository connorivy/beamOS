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
}
