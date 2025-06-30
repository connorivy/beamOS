using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.Common;

[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByName)]
public static partial class BeamOsDomainContractMappers
{
    public static BeamOs.StructuralAnalysis.Contracts.Common.Point ToContract(
        BeamOs.StructuralAnalysis.Domain.Common.Point source
    )
    {
        LengthUnit unit = source.X.Unit;
        return new(source.X.As(unit), source.Y.As(unit), source.Z.As(unit), unit.MapToContract());
    }

    //public static Dictionary<string, object> ToDict(CustomData customData) => customData.AsDict();

    public static UnitSettings ToDomain(this UnitSettingsContract source)
    {
        LengthUnit lengthUnit = source.LengthUnit.MapToLengthUnit();
        ForceUnit forceUnit = source.ForceUnit.MapToForceUnit();

        return new UnitSettings(
            lengthUnit,
            lengthUnit.ToArea(),
            lengthUnit.ToVolume(),
            forceUnit,
            forceUnit.DivideBy(lengthUnit),
            forceUnit.MultiplyBy(lengthUnit),
            forceUnit.GetPressure(lengthUnit),
            lengthUnit.ToAreaMomentOfInertiaUnit()
        );
    }

    public static partial ModelSettings ToContract(
        this Domain.PhysicalModel.ModelAggregate.ModelSettings source
    );

    public static UnitSettingsContract ToContract(this UnitSettings source)
    {
        return new()
        {
            LengthUnit = source.LengthUnit.MapToContract(),
            ForceUnit = source.ForceUnit.MapToContract(),
        };
    }

    public static partial AnalysisSettings ToContract(
        this Domain.PhysicalModel.ModelAggregate.AnalysisSettings source
    );

    public static partial Domain.PhysicalModel.ModelAggregate.ModelSettings ToDomain(
        this ModelSettings source
    );

    //public static BeamOs.StructuralAnalysis.Domain.Common.Point ToDomain(
    //    BeamOs.StructuralAnalysis.Contracts.Common.Point source
    //)
    //{
    //    return new(
    //        UnitsNetMappers.MapToContract(source.X),
    //        UnitsNetMappers.MapToContract(source.Y),
    //        UnitsNetMappers.MapToContract(source.Z)
    //    );
    //}

    public static MathNet.Spatial.Euclidean.Vector3D ToDomain(
        this BeamOs.StructuralAnalysis.Contracts.Common.Vector3 source
    )
    {
        return new(source.X, source.Y, source.Z);
    }

    public static ExisitingOrProposedGenericId ToGenericIdDomain(this ProposedID contract)
    {
        if (contract.ExistingId is not null)
        {
            return ExisitingOrProposedGenericId.FromExistingId(contract.ExistingId.Value);
        }
        else if (contract.ProposedId is not null)
        {
            return ExisitingOrProposedGenericId.FromProposedId(contract.ProposedId.Value);
        }
        throw new ArgumentNullException(
            nameof(contract),
            "Either ExistingId or ProposedId must be provided."
        );
    }

    public static ProposedID ToContract(this ExisitingOrProposedGenericId contract)
    {
        if (contract.ExistingId is not null)
        {
            return ProposedID.Existing(contract.ExistingId.Value);
        }
        else if (contract.ProposedId is not null)
        {
            return ProposedID.Proposed(contract.ProposedId.Value);
        }
        throw new ArgumentNullException(
            nameof(contract),
            "Either ExistingId or ProposedId must be provided."
        );
    }

    public static ExistingOrProposedNodeId ToNodeDomain(this ProposedID contract) =>
        ToNodeDomainOrNull(contract)
        ?? throw new ArgumentNullException(
            nameof(contract),
            "Either ExistingId or ProposedId must be provided."
        );

    public static ProposedID ToProposedIdContract<TId, TProposalId>(
        this ExistingOrProposedId<TId, TProposalId>? proposedId
    )
        where TId : struct, IIntBasedId
        where TProposalId : struct, IIntBasedId
    {
        if (proposedId?.ExistingId is not null)
        {
            return ProposedID.Existing(proposedId.ExistingId.Value.Id);
        }
        else if (proposedId?.ProposedId is not null)
        {
            return ProposedID.Proposed(proposedId.ProposedId.Value.Id);
        }
        return ProposedID.Default;
    }

    public static ProposedID? ToProposedIdContractOrNull<TId, TProposalId>(
        this ExistingOrProposedId<TId, TProposalId>? proposedId
    )
        where TId : struct, IIntBasedId
        where TProposalId : struct, IIntBasedId
    {
        if (proposedId?.ExistingId is not null)
        {
            return ProposedID.Existing(proposedId.ExistingId.Value.Id);
        }
        else if (proposedId?.ProposedId is not null)
        {
            return ProposedID.Proposed(proposedId.ProposedId.Value.Id);
        }
        return null;
    }

    public static ProposedID ToProposedIdContract(this ExistingOrProposedNodeId proposedId) =>
        ToProposedIdContract<NodeId, NodeProposalId>(proposedId);

    public static ProposedID? ToProposedIdContractWithNullable(
        this ExistingOrProposedNodeId? proposedId
    ) => ToProposedIdContractOrNull(proposedId);

    public static ProposedID ToProposedIdContract(this ExistingOrProposedMaterialId proposedId) =>
        ToProposedIdContract<MaterialId, MaterialProposalId>(proposedId);

    public static ProposedID? ToProposedIdContractWithNullable(
        this ExistingOrProposedMaterialId? proposedId
    ) => ToProposedIdContractOrNull(proposedId);

    public static ProposedID ToProposedIdContract(
        this ExistingOrProposedSectionProfileId proposedId
    ) => ToProposedIdContract<SectionProfileId, SectionProfileProposalId>(proposedId);

    public static ExistingOrProposedNodeId? ToNodeDomainOrNull(this ProposedID contract)
    {
        if (contract.ExistingId is not null)
        {
            return new((NodeId)contract.ExistingId);
        }
        else if (contract.ProposedId is not null)
        {
            return new((NodeProposalId)contract.ProposedId);
        }
        return null;
    }

    public static ExistingOrProposedMaterialId ToMaterialDomain(this ProposedID contract) =>
        ToMaterialDomainOrNull(contract)
        ?? throw new ArgumentNullException(
            nameof(contract),
            "Either ExistingId or ProposedId must be provided."
        );

    public static ExistingOrProposedMaterialId? ToMaterialDomainOrNull(this ProposedID contract)
    {
        if (contract.ExistingId is not null)
        {
            return new((MaterialId)contract.ExistingId);
        }
        else if (contract.ProposedId is not null)
        {
            return new((MaterialProposalId)contract.ProposedId);
        }
        return null;
    }

    public static ExistingOrProposedSectionProfileId ToSectionProfileDomain(
        this ProposedID contract
    ) =>
        ToSectionProfileDomainOrNull(contract)
        ?? throw new ArgumentNullException(
            nameof(contract),
            "Either ExistingId or ProposedId must be provided."
        );

    public static ExistingOrProposedSectionProfileId? ToSectionProfileDomainOrNull(
        this ProposedID contract
    )
    {
        if (contract.ExistingId is not null)
        {
            return new((SectionProfileId)contract.ExistingId);
        }
        else if (contract.ProposedId is not null)
        {
            return new((SectionProfileProposalId)contract.ProposedId);
        }
        return null;
    }
}
