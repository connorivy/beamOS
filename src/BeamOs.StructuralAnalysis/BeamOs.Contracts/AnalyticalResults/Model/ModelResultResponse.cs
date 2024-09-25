using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Common;

namespace BeamOs.Contracts.AnalyticalResults.Model;

public record ModelResultResponse(
    string ModelId,
    ForceContract MaxShear,
    ForceContract MinShear,
    TorqueContract MaxMoment,
    TorqueContract MinMoment
) : BeamOsContractBase;
