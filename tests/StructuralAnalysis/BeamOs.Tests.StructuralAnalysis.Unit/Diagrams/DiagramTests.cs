using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common.ValueObjects.DiagramBuilder;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using FluentAssertions;
using MathNet.Numerics;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Tests.StructuralAnalysis.Unit.Diagrams;

public class DiagramTests
{
    /// <summary>
    /// https://temple.manifoldapp.org/read/structural-analysis/section/072c774b-392a-4b99-a65d-f2e9d1fddfbd#:~:text=The%20moment%2Darea%20method%20uses,method%2C%20which%20are%20derived%20below.
    /// Example 7.6
    /// </summary>
    [Test]
    public void Test1()
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

        //shearDiagram.Intervals[0].Polynomial.Coefficients[0].Should().BeApproximately(96.25, 0.001);
        shearDiagram
            .Intervals[1]
            .Polynomial
            .Coefficients[0]
            .Should()
            .BeApproximately(96.25, 0.001);
        //Assert.Equal(shearDiagram.Intervals[0].Polynomial.Coefficients[0], 96.25);

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

        //Assert.Equal(-956.67, slopeDiagram.GetValueAtLocation(Length.Zero).ValueOnRight, 3);
        //Assert.Equal(
        //    -1785,
        //    deflectionDiagram.GetValueAtLocation(new Length(2, LengthUnit.Meter)).ValueOnLeft,
        //    1
        //);
        deflectionDiagram
            .GetValueAtLocation(new Length(2, LengthUnit.Meter))
            .ValueOnLeft
            .Should()
            .BeApproximately(-1785, 1);
    }
}
