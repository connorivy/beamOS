using BeamOS.Common.Domain.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.Element1DAggregate;
using BeamOS.DirectStiffnessMethod.Domain.Element1DAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.ModelAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.NodeAggregate;
using BeamOS.DirectStiffnessMethod.Domain.NodeAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.Services;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Services;
public class SolverServiceTests
{
    [Fact]
    public void SolveGivenSimpleBeamAndLoadsShouldEqualExpectedValue()
    {
        Restraints free2D = new(true, true, false, false, false, true);
        Node node0 = Node.Create(0, 16, 0, LengthUnit.Foot, free2D);
        node0.LinearLoads.Add(new(
            new Force(150, ForceUnit.KilopoundForce),
            DenseVector.OfArray([1, 0, 0])
            ));
        node0.LinearLoads.Add(new(
            new Force(300, ForceUnit.KilopoundForce),
            DenseVector.OfArray([0, -1, 0])
            ));
        Restraints pinned2d = new(false, false, false, false, false, true);
        Node node1 = Node.Create(-12, 0, 0, LengthUnit.Foot, pinned2d);
        Node node2 = Node.Create(0, 0, 0, LengthUnit.Foot, pinned2d);
        Node node3 = Node.Create(12, 0, 0, LengthUnit.Foot, pinned2d);

        Material steel29000ksi = new(
            new Pressure(29000, PressureUnit.KilopoundForcePerSquareInch),
            new Pressure(1, PressureUnit.KilopoundForcePerSquareInch)
        );
        SectionProfile area8in = new(
            new Area(8, AreaUnit.SquareInch),
            new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.InchToTheFourth),
            new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.InchToTheFourth),
            new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.InchToTheFourth)
        );
        SectionProfile area6in = new(
            new Area(6, AreaUnit.SquareInch),
            new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.InchToTheFourth),
            new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.InchToTheFourth),
            new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.InchToTheFourth)
        );
        var element0 = Element1D.Create(Angle.Zero, UnitSettings.K_IN, node0, node1, steel29000ksi, area8in);
        var element1 = Element1D.Create(Angle.Zero, UnitSettings.K_IN, node0, node2, steel29000ksi, area6in);
        var element2 = Element1D.Create(Angle.Zero, UnitSettings.K_IN, node0, node3, steel29000ksi, area8in);

        List<Element1D> element1Ds = [element0, element1, element2];
        Dictionary<NodeId, Node> nodes = new()
        {
            { node0.Id, node0 },
            { node1.Id, node1 },
            { node2.Id, node2 },
            { node3.Id, node3 },
        };
        SolverService.Solve(UnitSettings.K_IN, element1Ds, nodes);
    }
}
