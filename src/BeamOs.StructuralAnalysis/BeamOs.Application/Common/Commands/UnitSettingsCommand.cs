using UnitsNet.Units;

namespace BeamOs.Application.Common.Commands;

public record UnitSettingsCommand(
    LengthUnit LengthUnit,
    AreaUnit AreaUnit,
    VolumeUnit VolumeUnit,
    AreaMomentOfInertiaUnit AreaMomentOfInertiaUnit,
    ForceUnit ForceUnit,
    TorqueUnit TorqueUnit,
    ForcePerLengthUnit ForcePerLengthUnit,
    PressureUnit PressureUnit
);
