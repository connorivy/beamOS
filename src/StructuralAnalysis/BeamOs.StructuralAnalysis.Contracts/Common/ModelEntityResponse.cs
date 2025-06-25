using BeamOs.Common.Contracts;

namespace BeamOs.StructuralAnalysis.Contracts.Common;

public record ModelEntityResponse(int Id, Guid ModelId) : IModelEntity;

public enum BeamOsObjectType : byte
{
    Undefined = 0,
    Model = 1,
    Node = 2,
    InternalNode = 3,
    Element1d = 4,
    Material = 5,
    SectionProfile = 6,
    SectionProfileFromLibrary = 7,
    PointLoad = 50,
    MomentLoad = 51,
    DistributedLoad = 52,
    DistributedMomentLoad = 53,
    LoadCase = 70,
    LoadCombination = 71,
    ModelProposal = 100,
    NodeProposal = 101,
    InternalNodeProposal = 102,
    Element1dProposal = 103,
    MaterialProposal = 104,
    SectionProfileProposal = 105,
    Other = 255,
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

    public static BeamOsObjectType ToProposalType(this BeamOsObjectType beamOsObjectType)
    {
        return beamOsObjectType switch
        {
            BeamOsObjectType.Model => BeamOsObjectType.ModelProposal,
            BeamOsObjectType.Node => BeamOsObjectType.NodeProposal,
            BeamOsObjectType.Element1d => BeamOsObjectType.Element1dProposal,
            BeamOsObjectType.Material => BeamOsObjectType.MaterialProposal,
            BeamOsObjectType.SectionProfile => BeamOsObjectType.SectionProfileProposal,
            _ => throw new ArgumentOutOfRangeException(
                nameof(beamOsObjectType),
                $"The object type {beamOsObjectType} does not have a corresponding proposal type."
            ),
        };
    }

    public static BeamOsObjectType ToAffectedType(this BeamOsObjectType beamOsObjectType)
    {
        return beamOsObjectType switch
        {
            BeamOsObjectType.ModelProposal => BeamOsObjectType.Model,
            BeamOsObjectType.NodeProposal => BeamOsObjectType.Node,
            BeamOsObjectType.Element1dProposal => BeamOsObjectType.Element1d,
            BeamOsObjectType.MaterialProposal => BeamOsObjectType.Material,
            BeamOsObjectType.SectionProfileProposal => BeamOsObjectType.SectionProfile,
            _ => throw new ArgumentOutOfRangeException(
                nameof(beamOsObjectType),
                $"The object type {beamOsObjectType} does not have a corresponding affected type."
            ),
        };
    }

    public static bool IsProposalType(this BeamOsObjectType beamOsObjectType)
    {
        return beamOsObjectType switch
        {
            BeamOsObjectType.ModelProposal => true,
            BeamOsObjectType.NodeProposal => true,
            BeamOsObjectType.Element1dProposal => true,
            BeamOsObjectType.MaterialProposal => true,
            BeamOsObjectType.SectionProfileProposal => true,
            _ => false,
        };
    }
}
