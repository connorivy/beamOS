using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeamOS.Common.Domain.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.AnalyticalModelAggregate;
public class AnalyticalModelTests
{
    [Fact]
    public void RunAnalysis_ForSampleProblem_ShouldResultInExpectedValues()
    {
        Restraints free2D = new(true, true, false, false, false, true);
        AnalyticalNode node0 = AnalyticalNode.Create(0, 16, 0, LengthUnit.Foot, free2D);
        node0.LinearLoads.Add(new(
            new Force(150, ForceUnit.KilopoundForce),
            DenseVector.OfArray([1, 0, 0])
            ));
        node0.LinearLoads.Add(new(
            new Force(300, ForceUnit.KilopoundForce),
            DenseVector.OfArray([0, -1, 0])
            ));
        Restraints pinned2d = new(false, false, false, false, false, true);
        AnalyticalNode node1 = AnalyticalNode.Create(-12, 0, 0, LengthUnit.Foot, pinned2d);
        AnalyticalNode node2 = AnalyticalNode.Create(0, 0, 0, LengthUnit.Foot, pinned2d);
        AnalyticalNode node3 = AnalyticalNode.Create(12, 0, 0, LengthUnit.Foot, pinned2d);

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
        var element0 = AnalyticalElement1D.Create(Angle.Zero, UnitSettings.K_IN, node0, node1, steel29000ksi, area8in);
        var element1 = AnalyticalElement1D.Create(Angle.Zero, UnitSettings.K_IN, node0, node2, steel29000ksi, area6in);
        var element2 = AnalyticalElement1D.Create(Angle.Zero, UnitSettings.K_IN, node0, node3, steel29000ksi, area8in);

        List<AnalyticalElement1D> element1Ds = [element0, element1, element2];
        List<AnalyticalNode> nodes = [node0, node1, node2, node3];

        AnalyticalModel model = AnalyticalModel.RunAnalysis(UnitSettings.K_IN, element1Ds, nodes);
    }
}
