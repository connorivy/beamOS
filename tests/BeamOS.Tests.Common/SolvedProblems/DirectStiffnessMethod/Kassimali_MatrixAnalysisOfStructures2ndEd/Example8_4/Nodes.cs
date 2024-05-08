using BeamOs.Domain.Common.ValueObjects;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;
using UnitsNet.Units;

namespace BeamOS.Tests.Common.SolvedProblems.DirectStiffnessMethod.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

public class Nodes
{
    public static NodeFixture Node1 { get; } =
        new(new Point(0, 0, 0, LengthUnit.Foot), Restraint.Free);

    public static NodeFixture Node2 { get; } =
        new(new Point(-20, 0, 0, LengthUnit.Foot), Restraint.Fixed);

    public static NodeFixture Node3 { get; } =
        new(new Point(0, -20, 0, LengthUnit.Foot), Restraint.Fixed);

    public static NodeFixture Node4 { get; } =
        new(new Point(0, 0, -20, LengthUnit.Foot), Restraint.Fixed);

    public static NodeFixture[] All => [Node1, Node2, Node3, Node4];

    public static NodeResultFixture Node1ExpectedResult { get; } =
        new(
            Node1,
            new Forces(
                0,
                0,
                0,
                -1800,
                0,
                1800,
                ForceUnit.KilopoundForce,
                TorqueUnit.KilopoundForceInch
            ),
            new Displacements(
                -1.3522,
                -2.7965,
                -1.812,
                -3.0021,
                1.0569,
                6.4986,
                LengthUnit.Inch,
                AngleUnit.Radian
            )
        );

    public static NodeResultFixture Node2ExpectedResult { get; } =
        new(
            Node2,
            new Forces(
                5.3757,
                44.106,
                -.74070,
                2.1722,
                58987,
                2330.5,
                ForceUnit.KilopoundForce,
                TorqueUnit.KilopoundForceInch
            ),
            new Displacements(0, 0, 0, 0, 0, 0, LengthUnit.Inch, AngleUnit.Radian)
        );

    public static NodeResultFixture Node3ExpectedResult { get; } =
        new(
            Node3,
            new Forces(
                -4.6249,
                11.117,
                -6.4607,
                -515.55,
                -0.76472,
                369.67,
                ForceUnit.KilopoundForce,
                TorqueUnit.KilopoundForceInch
            ),
            new Displacements(0, 0, 0, 0, 0, 0, LengthUnit.Inch, AngleUnit.Radian)
        );

    public static NodeResultFixture Node4ExpectedResult { get; } =
        new(
            Node4,
            new Forces(
                -0.75082,
                4.7763,
                7.2034,
                -383.5,
                -60.166,
                -4.702,
                ForceUnit.KilopoundForce,
                TorqueUnit.KilopoundForceInch
            ),
            new Displacements(0, 0, 0, 0, 0, 0, LengthUnit.Inch, AngleUnit.Radian)
        );
}
