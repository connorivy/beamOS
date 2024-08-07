using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.UnitTests.DirectStiffnessMethod.Common.Factories;

internal static class DsmElement1dFactory
{
    public static DsmElement1d Create(
        Angle sectionProfileRotation = default,
        Pressure modulusOfElasticity = default,
        Pressure modulusOfRigidity = default,
        Area area = default,
        AreaMomentOfInertia strongAxisMomentOfInertia = default,
        AreaMomentOfInertia weakAxisMomentOfInertia = default,
        AreaMomentOfInertia polarMomentOfInertia = default,
        Line? baseLine = null,
        NodeId? startNodeId = null,
        NodeId? endNodeId = null,
        Element1DId? element1DId = null
    )
    {
        return new(
            element1DId ?? new(),
            sectionProfileRotation,
            modulusOfElasticity,
            modulusOfRigidity,
            area,
            strongAxisMomentOfInertia,
            weakAxisMomentOfInertia,
            polarMomentOfInertia,
            baseLine?.StartPoint ?? new(0, 0, 0, LengthUnit.Meter),
            baseLine?.EndPoint ?? new(1, 0, 0, LengthUnit.Meter),
            startNodeId ?? new(),
            endNodeId ?? new()
        );
    }

    public static DsmElement1d CreateWithUnitSiValues(
        Angle? sectionProfileRotation = default,
        Pressure? modulusOfElasticity = default,
        Pressure? modulusOfRigidity = default,
        Area? area = default,
        AreaMomentOfInertia? strongAxisMomentOfInertia = default,
        AreaMomentOfInertia? weakAxisMomentOfInertia = default,
        AreaMomentOfInertia? polarMomentOfInertia = default,
        Line? baseLine = null,
        NodeId? startNodeId = null,
        NodeId? endNodeId = null,
        Element1DId? element1DId = null
    )
    {
        return new(
            element1DId ?? new(),
            sectionProfileRotation ?? new(1, AngleUnit.Radian),
            modulusOfElasticity ?? new(1, PressureUnit.NewtonPerSquareMeter),
            modulusOfRigidity ?? new(1, PressureUnit.NewtonPerSquareMeter),
            area ?? new(1, AreaUnit.SquareMeter),
            strongAxisMomentOfInertia ?? new(1, AreaMomentOfInertiaUnit.MeterToTheFourth),
            weakAxisMomentOfInertia ?? new(1, AreaMomentOfInertiaUnit.MeterToTheFourth),
            polarMomentOfInertia ?? new(1, AreaMomentOfInertiaUnit.MeterToTheFourth),
            baseLine?.StartPoint ?? new(0, 0, 0, LengthUnit.Meter),
            baseLine?.EndPoint ?? new(1, 0, 0, LengthUnit.Meter),
            startNodeId ?? new(),
            endNodeId ?? new()
        );
    }
}
