using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using UnitsNet.Units;

namespace BeamOs.Tests.Common.SolvedProblems.SAP2000;

public partial class TwistyBowlFraming : ModelFixture, IHasExpectedNodeResults
{
    public override IEnumerable<LoadCase> LoadCaseRequests()
    {
        yield return new() { Name = "Load Case 1", Id = 1 };
    }

    public override IEnumerable<LoadCombination> LoadCombinationRequests()
    {
        yield return new LoadCombination(1, (1, 1.0));
        yield return new LoadCombination(2, (1, 1.0));
    }

    public override SourceInfo SourceInfo { get; } =
        new("SAP2000", FixtureSourceType.SAP2000, nameof(TwistyBowlFraming));

    public NodeResultFixture[] ExpectedNodeResults { get; } =
        [
            new()
            {
                ResultSetId = 1,
                NodeId = 1,
                DisplacementAlongX = new(.7591, LengthUnit.Inch),
                DisplacementAlongY = new(.0358, LengthUnit.Inch),
                DisplacementAlongZ = new(-.1152, LengthUnit.Inch),
            },
            new()
            {
                ResultSetId = 1,
                NodeId = 2,
                DisplacementAlongX = new(.968101, LengthUnit.Inch),
                DisplacementAlongY = new(.043022, LengthUnit.Inch),
                DisplacementAlongZ = new(-.119193, LengthUnit.Inch),
            },
            new()
            {
                ResultSetId = 1,
                NodeId = 3,
                DisplacementAlongX = new(.39517, LengthUnit.Inch),
                DisplacementAlongY = new(.02345, LengthUnit.Inch),
                DisplacementAlongZ = new(-.109077, LengthUnit.Inch),
            },
            new()
            {
                ResultSetId = 1,
                NodeId = 4,
                DisplacementAlongX = new(.027424, LengthUnit.Inch),
                DisplacementAlongY = new(.009371, LengthUnit.Inch),
                DisplacementAlongZ = new(-.086405, LengthUnit.Inch),
            },
            new()
            {
                ResultSetId = 1,
                NodeId = 5,
                ForceAlongX = new(.562, ForceUnit.KilopoundForce),
                ForceAlongY = new(-3.347, ForceUnit.KilopoundForce),
                ForceAlongZ = new(-1.363, ForceUnit.KilopoundForce),
            },
            new()
            {
                ResultSetId = 1,
                NodeId = 50,
                ForceAlongX = new(6.306, ForceUnit.KilopoundForce),
                ForceAlongY = new(10.784, ForceUnit.KilopoundForce),
                ForceAlongZ = new(1.566, ForceUnit.KilopoundForce),
            },
            new()
            {
                ResultSetId = 1,
                NodeId = 100,
                ForceAlongX = new(-.896, ForceUnit.KilopoundForce),
                ForceAlongY = new(2.552, ForceUnit.KilopoundForce),
                ForceAlongZ = new(-9.632, ForceUnit.KilopoundForce),
            },
            new()
            {
                ResultSetId = 1,
                NodeId = 150,
                DisplacementAlongX = new(-.60719, LengthUnit.Inch),
                DisplacementAlongY = new(.02335, LengthUnit.Inch),
                DisplacementAlongZ = new(-.4936, LengthUnit.Inch),
            },
            new()
            {
                ResultSetId = 1,
                NodeId = 200,
                ForceAlongX = new(-1.071, ForceUnit.KilopoundForce),
                ForceAlongY = new(12.826, ForceUnit.KilopoundForce),
                ForceAlongZ = new(-4.702, ForceUnit.KilopoundForce),
            },
            new()
            {
                ResultSetId = 1,
                NodeId = 300,
                DisplacementAlongX = new(-.0436, LengthUnit.Inch),
                DisplacementAlongY = new(-.75386, LengthUnit.Inch),
                DisplacementAlongZ = new(.54762, LengthUnit.Inch),
            },
            new()
            {
                ResultSetId = 1,
                NodeId = 330,
                DisplacementAlongX = new(1.90641, LengthUnit.Centimeter),
                DisplacementAlongY = new(-.04499, LengthUnit.Centimeter),
                DisplacementAlongZ = new(.57138, LengthUnit.Centimeter),
                RotationAboutX = new(-3.620e-4, AngleUnit.Radian),
                RotationAboutY = new(.00158, AngleUnit.Radian),
                RotationAboutZ = new(1.173e-4, AngleUnit.Radian),
            },
            new()
            {
                ResultSetId = 1,
                NodeId = 400,
                DisplacementAlongX = new(-.45241, LengthUnit.Inch),
                DisplacementAlongY = new(-.01802, LengthUnit.Inch),
                DisplacementAlongZ = new(-.53931, LengthUnit.Inch),
            },
            new()
            {
                ResultSetId = 1,
                NodeId = 500,
                DisplacementAlongX = new(.27826, LengthUnit.Inch),
                DisplacementAlongY = new(-.58624, LengthUnit.Inch),
                DisplacementAlongZ = new(-.79309, LengthUnit.Inch),
            },
        ];
}
