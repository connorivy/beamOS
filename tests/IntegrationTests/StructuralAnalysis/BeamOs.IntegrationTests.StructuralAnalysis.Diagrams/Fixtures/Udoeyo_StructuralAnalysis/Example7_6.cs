using BeamOs.Domain.Diagrams;
using BeamOs.Domain.Diagrams.Common.ValueObjects;
using BeamOs.Domain.Diagrams.Common.ValueObjects.DiagramBuilder;
using MathNet.Numerics;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.IntegrationTests.StructuralAnalysis.Diagrams.Fixtures.Udoeyo_StructuralAnalysis;

internal class Example7_6
    : ISolvedShearDiagramProblem,
        ISolvedMomentDiagramProblem,
        ISolvedDeflectionDiagramProblem
{
    public (
        Length length,
        DiagramValueAtLocation expectedValue
    )[] ExpectedShearDiagramValues { get; } =

        [
            (new Length(0, LengthUnit.Meter), 96.25),
            (new Length(8, LengthUnit.Meter), (-383.75, 50)),
            (new Length(10.9, LengthUnit.Meter), 50)
        ];

    private Diagram? shearDiagram;

    public Diagram GetShearDiagram()
    {
        this.shearDiagram ??= new DiagramBuilder(
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
        return this.shearDiagram;
    }

    private Diagram? momentDiagram;

    public Diagram GetMomentDiagram()
    {
        this.momentDiagram ??= this.GetShearDiagram()
            .Integrate()
            .AddPointLoads(new DiagramPointValue(new Length(2, LengthUnit.Meter), 40))
            .ApplyIntegrationBoundaryConditions(
                1,
                new DiagramPointValue(new Length(0, LengthUnit.Meter), 0),
                new DiagramPointValue(new Length(11, LengthUnit.Meter), 0)
            )
            .Build();
        return this.momentDiagram;
    }

    public (
        Length length,
        DiagramValueAtLocation expectedValue
    )[] ExpectedMomentDiagramValues { get; } =

        [
            (new Length(0, LengthUnit.Meter), 0),
            (new Length(8, LengthUnit.Meter), -150),
            (new Length(11, LengthUnit.Meter), 0)
        ];

    private Diagram? deflectionDiagram;

    public Diagram GetDeflectionDiagram()
    {
        this.deflectionDiagram ??= this.GetMomentDiagram()
            .Integrate()
            .Build()
            .Integrate()
            .ApplyIntegrationBoundaryConditions(
                2,
                new DiagramPointValue(new Length(0, LengthUnit.Meter), 0),
                new DiagramPointValue(new Length(8, LengthUnit.Meter), 0)
            )
            .Build();

        return this.deflectionDiagram;
    }

    public (
        Length length,
        DiagramValueAtLocation expectedValue
    )[] ExpectedDeflectionDiagramValues { get; } =

        [
            (new Length(0, LengthUnit.Meter), 0),
            (new Length(2, LengthUnit.Meter), -1785),
            (new Length(8, LengthUnit.Meter), 0)
        ];
}
