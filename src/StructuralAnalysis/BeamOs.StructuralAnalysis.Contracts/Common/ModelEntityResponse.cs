using BeamOs.Common.Contracts;

namespace BeamOs.StructuralAnalysis.Contracts.Common;

public record ModelEntityResponse(int Id, Guid ModelId) : IModelEntity;

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
    NodeProposal,
    Element1dProposal,
    MaterialProposal,
    SectionProfileProposal,
    Other = 1000,
}

public static class BeamOsObjectTypeExtensions
{
    public static BeamOsObjectType FromString(string type)
    {
        return type switch
        {
            nameof(BeamOsObjectType.Model) => BeamOsObjectType.Model,
            nameof(BeamOsObjectType.Node) => BeamOsObjectType.Node,
            nameof(BeamOsObjectType.Element1d) => BeamOsObjectType.Element1d,
            nameof(BeamOsObjectType.Material) => BeamOsObjectType.Material,
            nameof(BeamOsObjectType.SectionProfile) => BeamOsObjectType.SectionProfile,
            nameof(BeamOsObjectType.PointLoad) => BeamOsObjectType.PointLoad,
            nameof(BeamOsObjectType.MomentLoad) => BeamOsObjectType.MomentLoad,
            nameof(BeamOsObjectType.DistributedLoad) => BeamOsObjectType.DistributedLoad,
            nameof(BeamOsObjectType.DistributedMomentLoad) =>
                BeamOsObjectType.DistributedMomentLoad,
            nameof(BeamOsObjectType.LoadCase) => BeamOsObjectType.LoadCase,
            nameof(BeamOsObjectType.LoadCombination) => BeamOsObjectType.LoadCombination,
            nameof(BeamOsObjectType.ModelProposal) => BeamOsObjectType.ModelProposal,
            nameof(BeamOsObjectType.NodeProposal) => BeamOsObjectType.NodeProposal,
            nameof(BeamOsObjectType.Element1dProposal) => BeamOsObjectType.Element1dProposal,
            nameof(BeamOsObjectType.MaterialProposal) => BeamOsObjectType.MaterialProposal,
            nameof(BeamOsObjectType.SectionProfileProposal) =>
                BeamOsObjectType.SectionProfileProposal,
            _ => BeamOsObjectType.Undefined,
        };
    }

    public static bool TryParse(string type, out BeamOsObjectType beamOsObjectType)
    {
        beamOsObjectType = FromString(type);
        return beamOsObjectType != BeamOsObjectType.Undefined;
    }
}
