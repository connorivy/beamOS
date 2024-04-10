using BeamOs.Domain.Diagrams;
using BeamOs.Domain.Diagrams.Common.ValueObjects;
using BeamOs.Domain.Diagrams.Common.ValueObjects.DiagramBuilder;
using MathNet.Numerics;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.IntegrationTests.StructuralAnalysis.Diagrams.Fixtures.Hibbeler_StructuralAnalysis9thEd;

public class Example4_11 : SolvedDiagramProblem
{
    public override Diagram ShearDiagram =>
        new DiagramBuilder(
            new Length(32, LengthUnit.Foot),
            new Length(1, LengthUnit.Inch),
            LengthUnit.Foot
        )
            .AddPointLoads(
                new DiagramPointValue(new Length(0, LengthUnit.Foot), 4),
                new DiagramPointValue(new Length(16, LengthUnit.Foot), -5),
                new DiagramPointValue(new Length(20, LengthUnit.Foot), 45),
                new DiagramPointValue(new Length(32, LengthUnit.Foot), -6)
            )
            .AddDistributedLoads(
                new DiagramDistributedValue(0, 10, LengthUnit.Foot, new Polynomial(-2.0)),
                new DiagramDistributedValue(20, 26, LengthUnit.Foot, new Polynomial(.5 * 20, -.5)),
                new DiagramDistributedValue(26, 32, LengthUnit.Foot, new Polynomial(.5 * -32, .5))
            )
            .Build();

    public override (
        Length length,
        DiagramValueAtLocation expectedValue
    )[] ExpectedShearDiagramValues =>

        [
            (new Length(0, LengthUnit.Foot), 4),
            (new Length(2, LengthUnit.Foot), 0),
            (new Length(10, LengthUnit.Foot), -16),
            (new Length(16, LengthUnit.Foot), (-16, -21)),
            (new Length(20, LengthUnit.Foot), (-21, 24)),
            (new Length(32, LengthUnit.Foot), 6),
        ];

    public override Diagram? MomentDiagram =>
        this.ShearDiagram
            .Integrate()
            .ApplyIntegrationBoundaryConditions(
                1,
                new DiagramPointValue(new Length(0, LengthUnit.Foot), 60),
                new DiagramPointValue(new Length(32, LengthUnit.Foot), 0),
                new DiagramPointValue(new Length(10, LengthUnit.Foot), 0)
            )
            .Build();

    public override (
        Length length,
        DiagramValueAtLocation expectedValue
    )[] ExpectedMomentDiagramValues =>

        [
            (new Length(0, LengthUnit.Foot), 60),
            (new Length(2, LengthUnit.Foot), 64),
            (new Length(10, LengthUnit.Foot), 0),
            (new Length(16, LengthUnit.Foot), -96),
            (new Length(20, LengthUnit.Foot), -180),
            (new Length(32, LengthUnit.Foot), 0),
        ];
}
