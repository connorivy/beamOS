using BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.DsmNodeAggregate;
using UnitsNet;

namespace BeamOs.Application.DirectStiffnessMethod.AnalyticalElement1ds.Commands;

public record CreateAnalyticalElement1dGivenEntitiesCommand(
    Angle SectionProfileRotation,
    DsmNode StartNode,
    DsmNode EndNode,
    Material Material,
    SectionProfile SectionProfile
);
