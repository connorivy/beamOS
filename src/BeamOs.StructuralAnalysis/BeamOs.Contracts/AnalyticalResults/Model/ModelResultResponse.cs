using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Common;

namespace BeamOs.Contracts.AnalyticalResults.Model;

public record ModelResultResponse(
    string ModelId,
    UnitValueDto MaxShear,
    UnitValueDto MinShear,
    UnitValueDto MaxMoment,
    UnitValueDto MinMoment
) : BeamOsContractBase;
