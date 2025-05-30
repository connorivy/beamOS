using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public abstract class IndividualNodeVisitingRule : IModelRepairRule
{
    public void Apply(ModelProposalBuilder modelProposalBuilder, Length tolerance)
    {
        HashSet<NodeId> visitedNodeIds = [];
        foreach (Element1d element in modelProposalBuilder.Element1ds)
        {
            var (startNode, endNode) = modelProposalBuilder.GetStartAndEndNodes(element, out _);

            if (visitedNodeIds.Add(startNode.Id))
            {
                this.ApplyToSingleNode(modelProposalBuilder, tolerance, element, startNode);
            }
            if (visitedNodeIds.Add(endNode.Id))
            {
                this.ApplyToSingleNode(modelProposalBuilder, tolerance, element, endNode);
            }
        }
    }

    private void ApplyToSingleNode(
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance,
        Element1d element,
        Node node
    )
    {
        List<Node> nearbyNodes = modelProposalBuilder
            .Octree.FindNodesWithin(node.LocationPoint, tolerance.Meters, node.Id)
            .Select(modelProposalBuilder.ApplyExistingProposal)
            .Distinct()
            .Where(n => n.Id != node.Id)
            .ToList();

        List<Element1d> nearbyElement1ds = Element1dSpatialHelper.FindElement1dsWithin(
            modelProposalBuilder.Element1ds,
            modelProposalBuilder,
            node.LocationPoint,
            tolerance.Meters,
            node.Id
        );

        this.ApplyToSingleNode(
            element,
            node,
            nearbyNodes,
            nearbyElement1ds,
            modelProposalBuilder,
            tolerance
        );
    }

    protected abstract void ApplyToSingleNode(
        Element1d element,
        Node node,
        IList<Node> nearbyNodes,
        IList<Element1d> nearbyElement1ds,
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance
    );
}
