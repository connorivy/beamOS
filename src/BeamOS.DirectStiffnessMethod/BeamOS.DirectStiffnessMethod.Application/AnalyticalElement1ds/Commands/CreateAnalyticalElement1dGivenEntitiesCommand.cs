using BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Application.AnalyticalElement1ds.Commands;
public record CreateAnalyticalElement1dGivenEntitiesCommand(
    Angle SectionProfileRotation,
    AnalyticalNode StartNode,
    AnalyticalNode EndNode,
    Material Material,
    SectionProfile SectionProfile);
