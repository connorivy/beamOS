using BeamOs.Domain.Diagrams;
using BeamOs.Domain.Diagrams.Common.ValueObjects;
using UnitsNet;

namespace BeamOs.IntegrationTests.StructuralAnalysis.Diagrams.Fixtures;

public abstract class SolvedDiagramProblem
{
    public virtual Diagram? ShearDiagram { get; }

    public virtual (
        Length length,
        DiagramValueAtLocation expectedValue
    )[]? ExpectedShearDiagramValues { get; }

    public virtual Diagram? MomentDiagram { get; }

    public virtual (
        Length length,
        DiagramValueAtLocation expectedValue
    )[]? ExpectedMomentDiagramValues { get; }

    public virtual Diagram? RotationDiagram { get; }

    public virtual (
        Length length,
        DiagramValueAtLocation expectedValue
    )[]? ExpectedRotationDiagramValues { get; }

    public virtual Diagram? DeflectionDiagram { get; }

    public virtual (
        Length length,
        DiagramValueAtLocation expectedValue
    )[]? ExpectedDeflectionDiagramValues { get; }
}
