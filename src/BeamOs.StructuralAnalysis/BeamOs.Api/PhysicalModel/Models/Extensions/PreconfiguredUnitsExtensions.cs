using BeamOs.Contracts.Common;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Api.PhysicalModel.Models.Extensions;

public static class PreconfiguredUnitsExtensions
{
    public static UnitSettings ToDomainObject(this PreconfiguredUnits units)
    {
        return units switch
        {
            PreconfiguredUnits.N_M => UnitSettings.SI,
            PreconfiguredUnits.K_IN => UnitSettings.K_IN,
            PreconfiguredUnits.K_FT => UnitSettings.K_FT,
            _ => throw new NotImplementedException(),
        };
    }
}
