using System.Diagnostics;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair.Constraints;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public sealed class ModelProposalBuilder
{
    private readonly ModelProposal modelProposal;
    public ModelRepairOperationParameters ModelRepairOperationParameters { get; init; }
    public ModelProposalNodeStore NodeStore { get; }
    public ModelProposalElement1dStore Element1dStore { get; }

    public ModelProposalBuilder(
        ModelId modelId,
        string name,
        string description,
        ModelSettings settings,
        Octree octree,
        ModelRepairOperationParameters modelRepairOperationParameters,
        ModelProposalNodeStore nodeStore,
        ModelProposalElement1dStore element1dStore,
        ElementConstraintManager elementConstraintManager,
        ModelProposalId? id = null
    )
    {
        this.modelProposal = new(modelId, name, description, settings, id);
        this.Octree = octree;
        this.ModelRepairOperationParameters = modelRepairOperationParameters;
        this.NodeStore = nodeStore;
        this.Element1dStore = element1dStore;
        this.elementConstraintManager = elementConstraintManager;
    }

    private readonly ElementConstraintManager elementConstraintManager;
    private ModelId ModelId => this.modelProposal.ModelId;
    public ModelProposalId Id => this.modelProposal.Id;
    public ModelSettings Settings => this.modelProposal.Settings;
    public Octree Octree { get; }
    public IEnumerable<Element1d> Element1ds => this.Element1dStore.Values;
    public IEnumerable<NodeDefinition> Nodes => this.NodeStore.Values;

    public bool MergeNodes(NodeDefinition originalNode, NodeDefinition targetNode)
    {
        Debug.Assert(
            originalNode.Id != targetNode.Id,
            "Original and target nodes should not be the same"
        );
        Debug.Assert(
            !targetNode.DependsOnNode(originalNode.Id, this.Element1dStore, this.NodeStore),
            "Target node should not depend on original node"
        );
        List<Element1dProposal> element1dProposals = [];
        foreach (var originalElement1d in this.Element1ds)
        {
            var element1d = this.Element1dStore.ApplyExistingProposal(originalElement1d, out _);

            var (startNode, endNode) = this.GetStartAndEndNodes(element1d, out _);
            if (startNode.Id == originalNode.Id)
            {
                if (
                    !this.elementConstraintManager.NodeMovementSatisfiesElementConstraints(
                        element1d,
                        startNode,
                        endNode,
                        this.ModelRepairOperationParameters,
                        targetNode.GetLocationPoint(this.Element1dStore, this.NodeStore),
                        null
                    )
                )
                {
                    return false; // Movement does not satisfy constraints
                }

                element1dProposals.Add(
                    this.Element1dStore.CreateModifyElement1dProposal(
                        element1d,
                        this.Id,
                        startNodeId: targetNode.Id
                    )
                );
            }
            else if (endNode.Id == originalNode.Id)
            {
                if (
                    !this.elementConstraintManager.NodeMovementSatisfiesElementConstraints(
                        element1d,
                        startNode,
                        endNode,
                        this.ModelRepairOperationParameters,
                        null,
                        targetNode.GetLocationPoint(this.Element1dStore, this.NodeStore)
                    )
                )
                {
                    return false; // Movement does not satisfy constraints
                }

                element1dProposals.Add(
                    this.Element1dStore.CreateModifyElement1dProposal(
                        element1d,
                        this.Id,
                        endNodeId: targetNode.Id
                    )
                );
            }
        }

        foreach (var element1dProposal in element1dProposals)
        {
            this.Element1dStore.AddElement1dProposals(element1dProposal);
        }

        this.RemoveNode(originalNode);
        this.NodeStore.MergeNodes(originalNode, targetNode);
        return true;
    }

    public void RemoveNode(NodeDefinition node)
    {
        this.Octree.Remove(node.Id, node.GetLocationPoint(this.Element1dStore, this.NodeStore));
        this.NodeStore.RemoveNode(node);
        this.modelProposal.DeleteModelEntityProposals ??= [];
        this.modelProposal.DeleteModelEntityProposals.Add(
            new DeleteModelEntityProposal(this.ModelId, this.Id, node.Id, BeamOsObjectType.Node)
        );
    }

    public (NodeDefinition startNode, NodeDefinition endNode) GetStartAndEndNodes(
        Element1d element1d,
        out bool isModifiedInProposal
    )
    {
        var modifiedElement1d = this.Element1dStore.ApplyExistingProposal(
            element1d,
            out isModifiedInProposal
        );

        return (
            this.NodeStore.ApplyExistingProposal(modifiedElement1d.StartNodeId, out _),
            this.NodeStore.ApplyExistingProposal(modifiedElement1d.EndNodeId, out _)
        );
    }

    public (NodeDefinition startNode, NodeDefinition endNode) GetStartAndEndNodes(
        Element1d element1d
    ) => this.GetStartAndEndNodes(element1d, out _);

    public void AddMaterialProposals(MaterialProposal proposal) =>
        (this.modelProposal.MaterialProposals ??= []).Add(proposal);

    public void AddSectionProfileProposals(SectionProfileProposal proposal) =>
        (this.modelProposal.SectionProfileProposals ??= []).Add(proposal);

    public void AddSectionProfileProposalsFromLibrary(SectionProfileProposalFromLibrary proposal) =>
        (this.modelProposal.SectionProfileProposalsFromLibrary ??= []).Add(proposal);

    public void AddProposalIssues(ProposalIssue proposal) =>
        (this.modelProposal.ProposalIssues ??= []).Add(proposal);

    public ModelProposal Build()
    {
        this.modelProposal.NodeProposals = [.. this.NodeStore.GetNodeProposals()];
        this.modelProposal.InternalNodeProposals = [.. this.NodeStore.GetInternalNodeProposals()];
        this.modelProposal.Element1dProposals = [.. this.Element1dStore.GetElement1dProposals()];

        return this.modelProposal;
    }
}

public readonly record struct AxisAlignmentTolerance(
    AxisAlignmentToleranceLevel X,
    AxisAlignmentToleranceLevel Y,
    AxisAlignmentToleranceLevel Z
);

public enum AxisAlignmentToleranceLevel
{
    Undefined = 0,
    VeryStrict,
    Strict,
    Standard,
    Relaxed,
    VeryRelaxed,
}

public readonly record struct ModelRepairContext
{
    public required ModelProposalBuilder ModelProposalBuilder { get; init; }
    public required ModelRepairOperationParameters ModelRepairOperationParameters { get; init; }
}
