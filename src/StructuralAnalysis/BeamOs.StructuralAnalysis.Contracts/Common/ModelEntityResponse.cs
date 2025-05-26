using BeamOs.Common.Contracts;

namespace BeamOs.StructuralAnalysis.Contracts.Common;

public record ModelEntityResponse(int Id, Guid ModelId) : IModelEntity;

public record ModelEntityRequest(int Id, Guid ModelId) : IModelEntity;

public enum BeamOsObjectType
{
    Undefined = 0,
    Model,
    Node,
    Element1d,
    Material,
    SectionProfile,
    PointLoad,
    MomentLoad,
    DistributedLoad,
    DistributedMomentLoad,
    LoadCase,
    LoadCombination,
    ModelProposal,
}
