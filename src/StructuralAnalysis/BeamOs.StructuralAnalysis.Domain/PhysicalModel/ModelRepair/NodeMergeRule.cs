using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public class NodeMergeRule : IModelRepairRule
{
    public void Apply(
        Element1d element1D,
        Node startNode,
        Node endNode,
        IList<Node> nearbyStartNodes,
        IList<Element1d> element1DsCloseToStart,
        IList<Node> nearbyEndNodes,
        IList<Element1d> element1DsCloseToEnd,
        ModelProposalBuilder modelProposal,
        double tolerance
    )
    {
        // Merge close nodes in start region
        for (int i = 0; i < nearbyStartNodes.Count; i++)
        {
            for (int j = i + 1; j < nearbyStartNodes.Count; j++)
            {
                Node nodeA = nearbyStartNodes[i];
                Node nodeB = nearbyStartNodes[j];
                double distance = nodeA.LocationPoint.CalculateDistance(
                    nodeB.LocationPoint.X,
                    nodeB.LocationPoint.Y,
                    nodeB.LocationPoint.Z
                );
                if (distance < tolerance)
                {
                    double confidence = 1.0 - distance / tolerance;
                    NodeProposal proposal = new(
                        nodeA,
                        default,
                        nodeB.LocationPoint,
                        nodeB.Restraint
                    );
                    modelProposal.AddNodeProposal(proposal);
                }
            }
        }
        // Merge close nodes in end region
        for (int i = 0; i < nearbyEndNodes.Count; i++)
        {
            for (int j = i + 1; j < nearbyEndNodes.Count; j++)
            {
                Node nodeA = nearbyEndNodes[i];
                Node nodeB = nearbyEndNodes[j];
                double distance = nodeA.LocationPoint.CalculateDistance(
                    nodeB.LocationPoint.X,
                    nodeB.LocationPoint.Y,
                    nodeB.LocationPoint.Z
                );
                if (distance < tolerance)
                {
                    double confidence = 1.0 - distance / tolerance;
                    NodeProposal proposal = new(
                        nodeA,
                        default,
                        nodeB.LocationPoint,
                        nodeB.Restraint
                    );
                    modelProposal.AddNodeProposal(proposal);
                }
            }
        }
    }

    private static void NewMethod(
        IList<Node> nearbyNodes,
        ModelProposal modelProposal,
        double tolerance
    )
    {
        for (int i = 0; i < nearbyNodes.Count; i++)
        {
            for (int j = i + 1; j < nearbyNodes.Count; j++)
            {
                Node nodeA = nearbyNodes[i];
                Node nodeB = nearbyNodes[j];
                double distance = nodeA.LocationPoint.CalculateDistance(
                    nodeB.LocationPoint.X,
                    nodeB.LocationPoint.Y,
                    nodeB.LocationPoint.Z
                );
                if (distance < tolerance)
                {
                    double confidence = 1.0 - distance / tolerance;
                    NodeProposal proposal = new(
                        nodeA,
                        default,
                        nodeB.LocationPoint,
                        nodeB.Restraint
                    );
                    if (modelProposal.NodeProposals is null)
                    {
                        modelProposal.NodeProposals = [];
                    }
                    modelProposal.NodeProposals.Add(proposal);
                }
            }
        }
    }
}
