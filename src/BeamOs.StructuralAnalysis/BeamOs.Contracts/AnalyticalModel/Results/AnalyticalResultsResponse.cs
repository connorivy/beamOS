using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Common;

namespace BeamOs.Contracts.AnalyticalModel.Results;

public record AnalyticalResultsResponse(
    string ModelId,
    ForceContract MaxShear,
    ForceContract MinShear,
    TorqueContract MaxMoment,
    TorqueContract MinMoment
) : BeamOsContractBase;
