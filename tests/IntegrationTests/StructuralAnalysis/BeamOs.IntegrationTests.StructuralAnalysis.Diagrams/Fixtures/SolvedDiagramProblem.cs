using BeamOs.Domain.Diagrams;
using BeamOs.Domain.Diagrams.Common.ValueObjects;
using UnitsNet;

namespace BeamOs.IntegrationTests.StructuralAnalysis.Diagrams.Fixtures;

public interface ISolvedDiagramProblem { }

public interface ISolvedShearDiagramProblem : ISolvedDiagramProblem
{
    public Diagram GetShearDiagram();

    public (
        Length length,
        DiagramValueAtLocation expectedValue
    )[] ExpectedShearDiagramValues { get; }
}

public interface ISolvedMomentDiagramProblem : ISolvedDiagramProblem
{
    public Diagram GetMomentDiagram();

    public (
        Length length,
        DiagramValueAtLocation expectedValue
    )[] ExpectedMomentDiagramValues { get; }
}

public interface ISolvedRotationDiagramProblem : ISolvedDiagramProblem
{
    public Diagram GetRotationDiagram();

    public (
        Length length,
        DiagramValueAtLocation expectedValue
    )[] ExpectedRotationDiagramValues { get; }
}

public interface ISolvedDeflectionDiagramProblem : ISolvedDiagramProblem
{
    public Diagram GetDeflectionDiagram();

    public (
        Length length,
        DiagramValueAtLocation expectedValue
    )[] ExpectedDeflectionDiagramValues { get; }
}
