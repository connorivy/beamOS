using System.Diagnostics;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair.Rules;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public sealed class ModelProposalBuilder
{
    private readonly ModelProposal modelProposal;
    private readonly Dictionary<int, Node> nodeIdToNodeDict;
    public ModelRepairOperationParameters ModelRepairOperationParameters { get; init; }

    public ModelProposalBuilder(
        ModelId modelId,
        string name,
        string description,
        ModelSettings settings,
        Dictionary<int, Node> nodeIdToNodeDict,
        IList<Element1d> element1ds,
        Octree octree,
        ModelRepairOperationParameters modelRepairOperationParameters,
        ModelProposalId? id = null
    )
    {
        this.modelProposal = new(modelId, name, description, settings, id);
        this.nodeIdToNodeDict = nodeIdToNodeDict;
        this.Element1ds = element1ds;
        this.Octree = octree;
        this.ModelRepairOperationParameters = modelRepairOperationParameters;
    }

    private ModelId ModelId => this.modelProposal.ModelId;
    public ModelProposalId Id => this.modelProposal.Id;
    public ModelSettings Settings => this.modelProposal.Settings;
    public IList<Element1d> Element1ds { get; }
    public Octree Octree { get; }

    private readonly Dictionary<int, NodeProposal> modifyNodeProposalCache = [];
    private readonly Dictionary<int, int> mergedNodeToReplacedNodeIdDict = [];
    private readonly List<NodeProposal> newNodeProposals = [];

    public void MergeNodes(Node originalNode, Node targetNode)
    {
        Debug.Assert(
            originalNode.Id != targetNode.Id,
            "Original and target nodes should not be the same"
        );

        foreach (var element1d in this.Element1ds)
        {
            var (startNode, endNode) = this.GetStartAndEndNodes(element1d, out _);
            if (startNode.Id == originalNode.Id)
            {
                this.AddElement1dProposals(
                    new Element1dProposal(element1d, this.Id, startNodeId: targetNode.Id)
                );
            }
            else if (endNode.Id == originalNode.Id)
            {
                this.AddElement1dProposals(
                    new Element1dProposal(element1d, this.Id, endNodeId: targetNode.Id)
                );
            }
        }
        this.modelProposal.DeleteModelEntityProposals ??= [];
        this.modelProposal.DeleteModelEntityProposals.Add(
            new DeleteModelEntityProposal(
                this.ModelId,
                this.Id,
                originalNode.Id,
                BeamOsObjectType.Node
            )
        );

        this.mergedNodeToReplacedNodeIdDict[originalNode.Id] = targetNode.Id;
    }

    public void AddNodeProposal(NodeProposal proposal)
    {
        if (proposal.ExistingId is not null)
        {
            this.modifyNodeProposalCache[proposal.ExistingId.Value] = proposal;
        }
        else
        {
            this.newNodeProposals.Add(proposal);
        }
    }

    public Node ApplyExistingProposal(Node node, out bool isModifiedInProposal) =>
        this.ApplyExistingProposal(node.Id, out isModifiedInProposal);

    public Node ApplyExistingProposal(Node node) => this.ApplyExistingProposal(node.Id, out _);

    public Node ApplyExistingProposal(NodeId nodeId, out bool isModifiedInProposal)
    {
        if (this.mergedNodeToReplacedNodeIdDict.TryGetValue(nodeId.Id, out var replacedNodeId))
        {
            nodeId = replacedNodeId;
        }

        if (this.modifyNodeProposalCache.TryGetValue(nodeId, out var proposal))
        {
            isModifiedInProposal = true;
            return proposal.ToDomain();
        }

        isModifiedInProposal = false;
        return this.nodeIdToNodeDict[nodeId];
    }

    private readonly Dictionary<int, Element1dProposal> modifyElement1dProposalCache = [];
    private readonly List<Element1dProposal> newElement1dProposals = [];

    public void AddElement1dProposals(Element1dProposal proposal)
    {
        if (proposal.ExistingId is not null)
        {
            this.modifyElement1dProposalCache[proposal.ExistingId.Value] = proposal;
        }
        else
        {
            this.newElement1dProposals.Add(proposal);
        }
    }

    public Element1d ApplyExistingProposal(Element1d element1d, out bool isModifiedInProposal)
    {
        if (this.modifyElement1dProposalCache.TryGetValue(element1d.Id, out var proposal))
        {
            isModifiedInProposal = true;
            return proposal.ToDomain(null, null, null);
        }

        isModifiedInProposal = false;
        return element1d;
    }

    public (Node startNode, Node endNode) GetStartAndEndNodes(
        Element1d element1d,
        out bool isModifiedInProposal
    )
    {
        var modifiedElement1d = this.ApplyExistingProposal(element1d, out isModifiedInProposal);

        return (
            this.ApplyExistingProposal(modifiedElement1d.StartNodeId, out _),
            this.ApplyExistingProposal(modifiedElement1d.EndNodeId, out _)
        );
    }

    public (Node startNode, Node endNode) GetStartAndEndNodes(Element1d element1d) =>
        this.GetStartAndEndNodes(element1d, out _);

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
        this.modelProposal.NodeProposals =
        [
            .. this.modifyNodeProposalCache.Values,
            .. this.newNodeProposals,
        ];
        this.modelProposal.Element1dProposals =
        [
            .. this.modifyElement1dProposalCache.Values,
            .. this.newElement1dProposals,
        ];

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
