using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Common;

namespace BeamOs.Contracts.PhysicalModel.Element1d;

public record Element1DResponse(
    string Id,
    string ModelId,
    string StartNodeId,
    string EndNodeId,
    string MaterialId,
    string SectionProfileId,
    AngleContract SectionProfileRotation,
    Dictionary<string, object>? CustomData = null
//NodeResponse? StartNode = null,
//NodeResponse? EndNode = null,
//MaterialResponse? Material = null,
//SectionProfileResponse? SectionProfile = null
) : BeamOsEntityContractBase(Id);
