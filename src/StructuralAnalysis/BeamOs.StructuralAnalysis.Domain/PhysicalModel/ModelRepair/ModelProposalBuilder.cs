using System.Diagnostics;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using UnitsNet;

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
        ModelProposalId? id = null
    )
    {
        this.modelProposal = new(modelId, name, description, settings, id);
        this.Octree = octree;
        this.ModelRepairOperationParameters = modelRepairOperationParameters;
        this.NodeStore = nodeStore;
        this.Element1dStore = element1dStore;
    }

    private ModelId ModelId => this.modelProposal.ModelId;
    public ModelProposalId Id => this.modelProposal.Id;
    public ModelSettings Settings => this.modelProposal.Settings;
    public Octree Octree { get; }
    public IEnumerable<Element1d> Element1ds => this.Element1dStore.Values;
    public IEnumerable<NodeDefinition> Nodes => this.NodeStore.Values;

    public void MergeNodes(NodeDefinition originalNode, NodeDefinition targetNode)
    {
        Debug.Assert(
            originalNode.Id != targetNode.Id,
            "Original and target nodes should not be the same"
        );
        Debug.Assert(
            !targetNode.DependsOnNode(originalNode.Id, this.Element1dStore, this.NodeStore),
            "Target node should not depend on original node"
        );
        foreach (var originalElement1d in this.Element1ds)
        {
            var element1d = this.Element1dStore.ApplyExistingProposal(originalElement1d, out _);

            var (startNode, endNode) = this.GetStartAndEndNodes(element1d, out _);
            if (startNode.Id == originalNode.Id)
            {
                this.Element1dStore.AddElement1dProposals(
                    this.Element1dStore.CreateModifyElement1dProposal(
                        element1d,
                        this.Id,
                        startNodeId: targetNode.Id
                    )
                );
            }
            else if (endNode.Id == originalNode.Id)
            {
                this.Element1dStore.AddElement1dProposals(
                    this.Element1dStore.CreateModifyElement1dProposal(
                        element1d,
                        this.Id,
                        endNodeId: targetNode.Id
                    )
                );
            }
        }
        this.RemoveNode(originalNode);
        this.NodeStore.MergeNodes(originalNode, targetNode);
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

public record ModelRepairOperationParameters
{
    public required Length VeryRelaxedTolerance { get; init; }
    public required Length RelaxedTolerance { get; init; }
    public required Length StandardTolerance { get; init; }
    public required Length StrictTolerance { get; init; }
    public required Length VeryStrictTolerance { get; init; }
    public Angle VeryRelaxedAngleTolerance { get; init; } = new(10, AngleUnit.Degree);
    public Angle RelaxedAngleTolerance { get; init; } = new(10, AngleUnit.Degree);
    public Angle StandardAngleTolerance { get; init; } = new(5, AngleUnit.Degree);
    public Angle StrictAngleTolerance { get; init; } = new(2, AngleUnit.Degree);
    public Angle VeryStrictAngleTolerance { get; init; } = new(2, AngleUnit.Degree);

    public AxisAlignmentToleranceLevel GetAxisAlignmentToleranceLevel(Length length)
    {
        length = length.Abs();
        if (length < this.VeryStrictTolerance)
        {
            return AxisAlignmentToleranceLevel.VeryStrict;
        }
        if (length < this.StrictTolerance)
        {
            return AxisAlignmentToleranceLevel.Strict;
        }
        if (length < this.StandardTolerance)
        {
            return AxisAlignmentToleranceLevel.Standard;
        }
        if (length < this.RelaxedTolerance)
        {
            return AxisAlignmentToleranceLevel.Relaxed;
        }
        return AxisAlignmentToleranceLevel.VeryRelaxed;
    }

    public AxisAlignmentToleranceLevel GetAxisAlignmentToleranceLevel(Angle angle)
    {
        angle = angle.Abs();
        if (angle < this.VeryStrictAngleTolerance)
        {
            return AxisAlignmentToleranceLevel.VeryStrict;
        }
        if (angle < this.StrictAngleTolerance)
        {
            return AxisAlignmentToleranceLevel.Strict;
        }
        if (angle < this.StandardAngleTolerance)
        {
            return AxisAlignmentToleranceLevel.Standard;
        }
        if (angle < this.RelaxedAngleTolerance)
        {
            return AxisAlignmentToleranceLevel.Relaxed;
        }
        return AxisAlignmentToleranceLevel.VeryRelaxed;
    }

    public Length GetLengthTolerance(AxisAlignmentToleranceLevel level)
    {
        return level switch
        {
            AxisAlignmentToleranceLevel.VeryRelaxed => this.VeryRelaxedTolerance,
            AxisAlignmentToleranceLevel.Relaxed => this.RelaxedTolerance,
            AxisAlignmentToleranceLevel.Standard => this.StandardTolerance,
            AxisAlignmentToleranceLevel.Strict => this.StrictTolerance,
            AxisAlignmentToleranceLevel.VeryStrict => this.VeryStrictTolerance,
            AxisAlignmentToleranceLevel.Undefined or _ => throw new ArgumentOutOfRangeException(
                nameof(level),
                level,
                null
            ),
        };
    }

    public Angle GetAngleTolerance(AxisAlignmentToleranceLevel level)
    {
        return level switch
        {
            AxisAlignmentToleranceLevel.VeryRelaxed => this.VeryRelaxedAngleTolerance,
            AxisAlignmentToleranceLevel.Relaxed => this.RelaxedAngleTolerance,
            AxisAlignmentToleranceLevel.Standard => this.StandardAngleTolerance,
            AxisAlignmentToleranceLevel.Strict => this.StrictAngleTolerance,
            AxisAlignmentToleranceLevel.VeryStrict => this.VeryStrictAngleTolerance,
            AxisAlignmentToleranceLevel.Undefined or _ => throw new ArgumentOutOfRangeException(
                nameof(level),
                level,
                null
            ),
        };
    }

    public AxisAlignmentTolerance GetAxisAlignmentTolerance(Node startNode, Node endNode)
    {
        return new(
            this.GetAxisAlignmentToleranceLevel(
                startNode.LocationPoint.X - endNode.LocationPoint.X
            ),
            this.GetAxisAlignmentToleranceLevel(
                startNode.LocationPoint.Y - endNode.LocationPoint.Y
            ),
            this.GetAxisAlignmentToleranceLevel(startNode.LocationPoint.Z - endNode.LocationPoint.Z)
        );
    }

    public AxisAlignmentTolerance GetAxisAlignmentTolerance(
        Common.Point startNodeLocation,
        Common.Point endNodeLocation
    )
    {
        return new(
            this.GetAxisAlignmentToleranceLevel(startNodeLocation.X - endNodeLocation.X),
            this.GetAxisAlignmentToleranceLevel(startNodeLocation.Y - endNodeLocation.Y),
            this.GetAxisAlignmentToleranceLevel(startNodeLocation.Z - endNodeLocation.Z)
        );
    }

    internal Length GetToleranceForRule(IModelRepairRule rule) =>
        rule.RuleType switch
        {
            ModelRepairRuleType.Favorable => this.RelaxedTolerance,
            ModelRepairRuleType.Standard => this.StandardTolerance,
            ModelRepairRuleType.Unfavorable => this.StrictTolerance,
            ModelRepairRuleType.Undefined or _ => throw new ArgumentOutOfRangeException(
                nameof(rule),
                rule,
                null
            ),
        };

    internal Length GetTolerance(ModelRepairRuleType ruleType) =>
        ruleType switch
        {
            ModelRepairRuleType.Favorable => this.RelaxedTolerance,
            ModelRepairRuleType.Standard => this.StandardTolerance,
            ModelRepairRuleType.Unfavorable => this.StrictTolerance,
            ModelRepairRuleType.Undefined or _ => throw new ArgumentOutOfRangeException(
                nameof(ruleType),
                ruleType,
                null
            ),
        };
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

public sealed record ModelRepairContext
{
    public required ModelProposalBuilder ModelProposalBuilder { get; init; }
    public required ModelRepairOperationParameters ModelRepairOperationParameters { get; init; }
}
