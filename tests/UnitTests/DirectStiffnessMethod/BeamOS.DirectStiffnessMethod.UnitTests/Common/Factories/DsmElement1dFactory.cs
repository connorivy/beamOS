using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Factories;

internal static class DsmElement1dFactory
{
    public static DsmElement1dVo Create(
        Angle sectionProfileRotation = default,
        Pressure modulusOfElasticity = default,
        Pressure modulusOfRigidity = default,
        Area area = default,
        AreaMomentOfInertia strongAxisMomentOfInertia = default,
        AreaMomentOfInertia weakAxisMomentOfInertia = default,
        AreaMomentOfInertia polarMomentOfInertia = default,
        Line? baseLine = null,
        NodeId? startNodeId = null,
        NodeId? endNodeId = null
    )
    {
        return new(
            sectionProfileRotation,
            modulusOfElasticity,
            modulusOfRigidity,
            area,
            strongAxisMomentOfInertia,
            weakAxisMomentOfInertia,
            polarMomentOfInertia,
            baseLine,
            startNodeId ?? new(),
            endNodeId ?? new()
        );
    }

    public static DsmElement1dVo CreateWithUnitSiValues(
        Angle? sectionProfileRotation = default,
        Pressure? modulusOfElasticity = default,
        Pressure? modulusOfRigidity = default,
        Area? area = default,
        AreaMomentOfInertia? strongAxisMomentOfInertia = default,
        AreaMomentOfInertia? weakAxisMomentOfInertia = default,
        AreaMomentOfInertia? polarMomentOfInertia = default,
        Line? baseLine = null,
        NodeId? startNodeId = null,
        NodeId? endNodeId = null
    )
    {
        return new(
            sectionProfileRotation ?? new(1, AngleUnit.Radian),
            modulusOfElasticity ?? new(1, PressureUnit.NewtonPerSquareMeter),
            modulusOfRigidity ?? new(1, PressureUnit.NewtonPerSquareMeter),
            area ?? new(1, AreaUnit.SquareMeter),
            strongAxisMomentOfInertia ?? new(1, AreaMomentOfInertiaUnit.MeterToTheFourth),
            weakAxisMomentOfInertia ?? new(1, AreaMomentOfInertiaUnit.MeterToTheFourth),
            polarMomentOfInertia ?? new(1, AreaMomentOfInertiaUnit.MeterToTheFourth),
            baseLine ?? new(0, 0, 0, 1, 0, 0, LengthUnit.Meter),
            startNodeId ?? new(),
            endNodeId ?? new()
        );
    }
}
