using BeamOs.Domain.Common.ValueObjects;
using BeamOS.Tests.Common.Fixtures;
using UnitsNet.Units;

namespace BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

public class Kassimali_Example8_4_Nodes
{
    public static NodeFixture2 Node1 { get; } =
        new()
        {
            LocationPoint = new Point(0, 0, 0, LengthUnit.Foot),
            Restraint = Restraint.Free,
            ModelId = Kassimali_Example8_4.IdStatic,
            PointLoads =  [Kassimali_Example8_4_PointLoads.PointLoad1],
            MomentLoads =
            [
                Kassimali_Example8_4_MomentLoads.MomentLoad1,
                Kassimali_Example8_4_MomentLoads.MomentLoad2,
                Kassimali_Example8_4_MomentLoads.MomentLoad3
            ],
        };

    public static NodeFixture2 Node2 { get; } =
        new()
        {
            LocationPoint = new Point(-20, 0, 0, LengthUnit.Foot),
            Restraint = Restraint.Fixed,
            ModelId = Kassimali_Example8_4.IdStatic,
            PointLoads =  [Kassimali_Example8_4_PointLoads.PointLoad2],
            MomentLoads =  [Kassimali_Example8_4_MomentLoads.MomentLoad4]
        };

    public static NodeFixture2 Node3 { get; } =
        new()
        {
            LocationPoint = new Point(0, -20, 0, LengthUnit.Foot),
            Restraint = Restraint.Fixed,
            ModelId = Kassimali_Example8_4.IdStatic
        };

    public static NodeFixture2 Node4 { get; } =
        new()
        {
            LocationPoint = new Point(0, 0, -20, LengthUnit.Foot),
            Restraint = Restraint.Fixed,
            ModelId = Kassimali_Example8_4.IdStatic
        };

    public static NodeFixture2[] All { get; } = [Node1, Node2, Node3, Node4];

    public static NodeResultFixture2 Node1ExpectedResult { get; } =
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

    public static NodeResultFixture2 Node2ExpectedResult { get; } =
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

    public static NodeResultFixture2 Node3ExpectedResult { get; } =
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

    public static NodeResultFixture2 Node4ExpectedResult { get; } =
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
