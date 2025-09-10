using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public abstract class IndividualNodeVisitingRule(ModelRepairContext context) : IModelRepairRule
{
    public abstract ModelRepairRuleType RuleType { get; }
    protected ModelRepairContext Context => context;

    public void Apply()
    {
        HashSet<NodeId> visitedNodeIds = [];
        foreach (Element1d element in this.Context.ModelProposalBuilder.Element1ds)
        {
            var (startNode, endNode) = this.Context.ModelProposalBuilder.GetStartAndEndNodes(
                element,
                out _
            );

            if (visitedNodeIds.Add(startNode.Id))
            {
                var startNodeLocation = startNode.GetLocationPoint(
                    this.Context.ModelProposalBuilder.Element1dStore,
                    this.Context.ModelProposalBuilder.NodeStore
                );
                this.ApplyToSingleNode(
                    this.Context.ModelProposalBuilder,
                    this.Context.ModelRepairOperationParameters.GetTolerance(this.RuleType),
                    element,
                    startNode,
                    startNodeLocation
                );
            }
            if (visitedNodeIds.Add(endNode.Id))
            {
                var endNodeLocation = endNode.GetLocationPoint(
                    this.Context.ModelProposalBuilder.Element1dStore,
                    this.Context.ModelProposalBuilder.NodeStore
                );
                this.ApplyToSingleNode(
                    this.Context.ModelProposalBuilder,
                    this.Context.ModelRepairOperationParameters.GetTolerance(this.RuleType),
                    element,
                    endNode,
                    endNodeLocation
                );
            }
        }
    }

    private void ApplyToSingleNode(
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance,
        Element1d element,
        NodeDefinition node,
        Point nodeLocation
    )
    {
        var nearbyNodes = modelProposalBuilder
            .Octree.FindNodeIdsWithin(nodeLocation, tolerance.Meters, node.Id)
            .Select(modelProposalBuilder.NodeStore.ApplyExistingProposal)
            .Distinct()
            .Where(n => n.Id != node.Id)
            .ToList();

        List<Element1d> nearbyElement1ds = Element1dSpatialHelper.FindElement1dsWithin(
            modelProposalBuilder.Element1ds,
            modelProposalBuilder,
            nodeLocation,
            tolerance.Meters,
            node.Id
        );

        this.ApplyToSingleNode(
            element,
            node,
            nodeLocation,
            nearbyNodes.OfType<Node>().ToList(),
            nearbyNodes.OfType<InternalNode>().ToList(),
            nearbyElement1ds,
            tolerance
        );
    }

    protected abstract void ApplyToSingleNode(
        Element1d element,
        NodeDefinition node,
        Point nodeLocation,
        IList<Node> nearbyNodes,
        IList<InternalNode> nearbyInternalNodes,
        IList<Element1d> nearbyElement1ds,
        Length tolerance
    );
}
