using System.Diagnostics;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;

internal sealed class Element1dProposal : BeamOsModelProposalEntity<Element1dProposalId, Element1dId>
{
    public Element1dProposal(
        ModelId modelId,
        ModelProposalId modelProposalId,
        ExistingOrProposedNodeId startNodeId,
        ExistingOrProposedNodeId endNodeId,
        ExistingOrProposedMaterialId materialId,
        ExistingOrProposedSectionProfileId sectionProfileId,
        Element1dId? existingId = null,
        Element1dProposalId? id = null
    )
        : base(id ?? new(), modelProposalId, modelId, existingId)
    {
        this.StartNodeId = startNodeId;
        this.EndNodeId = endNodeId;
        this.MaterialId = materialId;
        this.SectionProfileId = sectionProfileId;
    }

    public Element1dProposal(
        Element1d element1d,
        ModelProposalId modelProposalId,
        ExistingOrProposedNodeId? startNodeId = null,
        ExistingOrProposedNodeId? endNodeId = null,
        ExistingOrProposedMaterialId? materialId = null,
        ExistingOrProposedSectionProfileId? sectionProfileId = null,
        Element1dProposalId? id = null
    )
        : this(
            element1d.ModelId,
            modelProposalId,
            startNodeId ?? new(element1d.StartNodeId),
            endNodeId ?? new(element1d.EndNodeId),
            materialId ?? new(element1d.MaterialId),
            sectionProfileId ?? new(element1d.SectionProfileId),
            element1d.Id,
            id
        ) { }

    public Element1dProposal(
        Element1dProposal element1dProposal,
        ExistingOrProposedNodeId? startNodeId = null,
        ExistingOrProposedNodeId? endNodeId = null,
        ExistingOrProposedMaterialId? materialId = null,
        ExistingOrProposedSectionProfileId? sectionProfileId = null,
        Element1dProposalId? id = null
    )
        : this(
            element1dProposal.ModelId,
            element1dProposal.ModelProposalId,
            startNodeId ?? element1dProposal.StartNodeId,
            endNodeId ?? element1dProposal.EndNodeId,
            materialId ?? element1dProposal.MaterialId,
            sectionProfileId ?? element1dProposal.SectionProfileId,
            element1dProposal.ExistingId,
            id
        ) { }

    public ExistingOrProposedNodeId StartNodeId { get; private set; }
    public ExistingOrProposedNodeId EndNodeId { get; private set; }
    public ExistingOrProposedMaterialId MaterialId { get; private set; }
    public ExistingOrProposedSectionProfileId SectionProfileId { get; private set; }

    public Element1d ToDomain(
        Dictionary<NodeProposalId, NodeDefinition>? nodeProposalIdToNewIdDict,
        Dictionary<MaterialProposalId, Material>? materialProposalIdToNewIdDict,
        Dictionary<
            SectionProfileProposalId,
            SectionProfileInfoBase
        >? sectionProfileProposalIdToNewIdDict
    )
    {
        var (startNodeId, startNode) = this.StartNodeId.ToIdAndEntity(nodeProposalIdToNewIdDict);
        var (endNodeId, endNode) = this.EndNodeId.ToIdAndEntity(nodeProposalIdToNewIdDict);
        var (materialId, material) = this.MaterialId.ToIdAndEntity(materialProposalIdToNewIdDict);
        var (sectionProfileId, sectionProfile) = this.SectionProfileId.ToIdAndEntity(
            sectionProfileProposalIdToNewIdDict
        );
        return new(
            this.ModelId,
            startNodeId,
            endNodeId,
            materialId,
            sectionProfileId,
            this.ExistingId
        )
        {
            StartNode = startNode,
            EndNode = endNode,
            Material = material,
            SectionProfile = sectionProfile,
        };
    }

    [Obsolete("EF Core Constructor")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Element1dProposal()
        : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
