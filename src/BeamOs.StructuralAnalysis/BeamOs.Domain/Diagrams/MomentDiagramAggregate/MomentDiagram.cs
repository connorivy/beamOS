using BeamOs.Domain.Diagrams.Common;
using BeamOs.Domain.Diagrams.DiagramBaseAggregate;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate.ValueObjects;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.Diagrams.MomentDiagramAggregate;

public sealed class MomentDiagram : DiagramBase<ShearForceDiagramId>
{
    internal MomentDiagram(
        Length elementLength,
        LengthUnit lengthUnit,
        List<DiagramConsistantInterval> intervals,
        ShearForceDiagramId? id = null
    )
        : base(elementLength, lengthUnit, intervals, id ?? new()) { }
}
