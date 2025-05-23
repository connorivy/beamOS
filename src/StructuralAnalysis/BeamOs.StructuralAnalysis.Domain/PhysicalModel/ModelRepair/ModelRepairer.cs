using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public class ModelRepairer
{
    private readonly IList<Node> nodes;
    private readonly IList<Element1d> element1ds;
    private readonly double tolerance;
    private static readonly List<IModelRepairRule> rules =
    [
        new NodeMergeRule(),
        new Element1dExtendOrShortenRule(),
    ];
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
        this.octree = octree;
    }

    public ModelProposal ProposeRepairs(Model model)
    {
        ModelProposalBuilder modelProposal = new(
            model.Id,
            "Repair Proposal",
            "Proposed repairs for model connectivity",
            model.Settings,
            this.nodes.ToDictionary(node => node.Id.Id)
        );

        foreach (IModelRepairRule rule in rules)
        {
            foreach (Element1d element in this.element1ds)
            {
                var (startNode, endNode) = modelProposal.GetStartAndEndNodes(element, out _);

                List<Node> nearbyStartNodes = this.octree.FindNodesWithin(
                    startNode.LocationPoint,
                    this.tolerance,
                    startNode.Id
                );
                List<Element1d> nearbyStartElements = Element1dSpatialHelper.FindElement1dsWithin(
                    this.element1ds,
                    startNode.LocationPoint,
                    this.tolerance
                );
                List<Node> nearbyEndNodes = this.octree.FindNodesWithin(
                    endNode.LocationPoint,
                    this.tolerance,
                    endNode.Id
                );
                List<Element1d> nearbyEndElements = Element1dSpatialHelper.FindElement1dsWithin(
                    this.element1ds,
                    endNode.LocationPoint,
                    this.tolerance
                );

                rule.Apply(
                    element,
                    startNode,
                    endNode,
                    nearbyStartNodes,
                    nearbyStartElements,
                    nearbyEndNodes,
                    nearbyEndElements,
                    modelProposal,
                    this.tolerance
                );
            }
        }
        return modelProposal.Build();
    }
}
