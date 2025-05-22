using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

public interface IModelRepairRule
{
    public void Apply(
        IList<Node> nearbyStartNodes,
        IList<Element1d> element1DsCloseToStart,
        IList<Node> nearbyEndNodes,
        IList<Element1d> element1DsCloseToEnd,
        ModelProposal modelProposal,
        double tolerance
    );
}

public class NodeMergeRule : IModelRepairRule
{
    public void Apply(
        IList<Node> nearbyStartNodes,
        IList<Element1d> element1DsCloseToStart,
        IList<Node> nearbyEndNodes,
        IList<Element1d> element1DsCloseToEnd,
        ModelProposal modelProposal,
        double tolerance
    ) { }
}

public class ElementEndpointRule : IModelRepairRule
{
    public void Apply(
        IList<Node> nearbyStartNodes,
        IList<Element1d> element1DsCloseToStart,
        IList<Node> nearbyEndNodes,
        IList<Element1d> element1DsCloseToEnd,
        ModelProposal modelProposal,
        double tolerance
    ) { }
}

public class ModelRepairer
{
    private readonly IList<Node> nodes;
    private readonly IList<Element1d> element1ds;
    private readonly double tolerance;
    private readonly List<IModelRepairRule> rules;
    private readonly Octree octree;

    public ModelRepairer(
        IList<Node> nodes,
        IList<Element1d> element1ds,
        double tolerance,
        Octree octree
    )
    {
        this.nodes = nodes;
        this.element1ds = element1ds;
        this.tolerance = tolerance;
        this.rules = new List<IModelRepairRule> { new NodeMergeRule(), new ElementEndpointRule() };
        this.octree = octree;
    }

    public ModelProposal ProposeRepairs(Model model)
    {
        List<NodeProposal> nodeProposals = [];
        List<Element1dProposal> element1dProposals = [];
        ModelProposal modelProposal = new(
            model,
            "Repair Proposal",
            "Proposed repairs for model connectivity",
            model.Settings
        );
        foreach (Element1d element in this.element1ds)
        {
            Node? startNode = element.StartNode;
            Node? endNode = element.EndNode;
            // Find nearby nodes for start and end node
            List<Node> nearbyStartNodes = this.octree.FindNodesWithin(
                startNode.LocationPoint,
                this.tolerance
            );
            List<Element1d> nearbyStartElements = Element1dSpatialHelper.FindElement1dsWithin(
                this.element1ds,
                startNode.LocationPoint,
                this.tolerance
            );
            List<Node> nearbyEndNodes = this.octree.FindNodesWithin(
                endNode.LocationPoint,
                this.tolerance
            );
            List<Element1d> nearbyEndElements = Element1dSpatialHelper.FindElement1dsWithin(
                this.element1ds,
                endNode.LocationPoint,
                this.tolerance
            );

            foreach (IModelRepairRule rule in this.rules)
            {
                rule.Apply(
                    nearbyStartNodes,
                    nearbyStartElements,
                    nearbyEndNodes,
                    nearbyEndElements,
                    modelProposal,
                    this.tolerance
                );
            }
        }
        return modelProposal;
    }
}
