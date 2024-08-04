using BeamOS.Tests.Common.Fixtures;
using UnitsNet.Units;

namespace BeamOS.Tests.Common.SolvedProblems.ETABS_Models.Simple_3_Story_Diagonal;

public partial class Simple_3_Story_Diagonal : IHasExpectedNodeDisplacementResults
{
    public NodeResultFixture[] ExpectedNodeDisplacementResults { get; } =

        [
            new NodeResultFixture()
            {
                NodeId = NodeLocationString(72, 36, -48),
                DisplacementAlongX = new(-3.309, LengthUnit.Inch),
                DisplacementAlongY = new(.000487, LengthUnit.Inch),
                DisplacementAlongZ = new(2.49338, LengthUnit.Inch),
                ForceAlongX = new(18.848, ForceUnit.KilopoundForce),
                ForceAlongY = new(-5.653, ForceUnit.KilopoundForce),
                ForceAlongZ = new(-2.252, ForceUnit.KilopoundForce),
                TorqueAboutX = new(-14.800, TorqueUnit.KilopoundForceFoot),
                TorqueAboutY = new(.0108, TorqueUnit.KilopoundForceFoot),
                TorqueAboutZ = new(-186.8722, TorqueUnit.KilopoundForceFoot),
            },
            new NodeResultFixture()
            {
                NodeId = NodeLocationString(72, 36, -24),
                DisplacementAlongX = new(-3.694, LengthUnit.Inch),
                DisplacementAlongY = new(-.005209, LengthUnit.Inch),
                DisplacementAlongZ = new(2.49356, LengthUnit.Inch),
            },
            new NodeResultFixture()
            {
                NodeId = NodeLocationString(72, 36, 0),
                DisplacementAlongX = new(-3.737, LengthUnit.Inch),
                DisplacementAlongY = new(-.00249, LengthUnit.Inch),
                DisplacementAlongZ = new(2.49369, LengthUnit.Inch),
            },
            new NodeResultFixture()
            {
                NodeId = NodeLocationString(72, 36, 24),
                DisplacementAlongX = new(-3.88834, LengthUnit.Inch),
                DisplacementAlongY = new(-.013186, LengthUnit.Inch),
                DisplacementAlongZ = new(2.49452, LengthUnit.Inch),
                RotationAboutX = new(.001589, AngleUnit.Radian),
                RotationAboutY = new(-.006357, AngleUnit.Radian),
                RotationAboutZ = new(.005631, AngleUnit.Radian),
                ForceAlongX = new(2.770, ForceUnit.KilopoundForce),
                ForceAlongY = new(-0.454, ForceUnit.KilopoundForce),
                ForceAlongZ = new(10.645, ForceUnit.KilopoundForce),
            }
        ];
}
