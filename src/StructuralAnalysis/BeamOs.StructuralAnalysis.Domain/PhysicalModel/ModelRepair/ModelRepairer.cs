using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public class ModelRepairer
{
    private readonly IList<Node> nodes;
    private readonly IList<Element1d> element1ds;
    private readonly Length tolerance;
    private static readonly List<IModelRepairRule> rules =
    [
        new Element1dExtendOrShortenRule(),
        new NodeMergeRule(),
        new NodeSnapToElement1dRule(),
    ];
    private readonly Octree octree;

    public ModelRepairer(
        IList<Node> nodes,
        IList<Element1d> element1ds,
        Length tolerance,
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
            this.nodes.ToDictionary(node => node.Id.Id),
            this.element1ds
        );

        for (int i = 0; i < 3; i++)
        {
            var tolerance = this.tolerance * (i + 1) / 3.0;
            foreach (IModelRepairRule rule in rules)
            {
                HashSet<NodeId> visitedNodeIds = [];
                foreach (Element1d element in this.element1ds)
                {
                    var (startNode, endNode) = modelProposal.GetStartAndEndNodes(element, out _);
                    bool startNodeVisited = !visitedNodeIds.Add(startNode.Id);
                    bool endNodeVisited = !visitedNodeIds.Add(endNode.Id);

                    if (startNodeVisited && endNodeVisited)
                    {
                        // Both nodes are already visited, skip
                        continue;
                    }
                    if (endNodeVisited)
                    {
                        // Only end node is visited, apply rule to start node
                        rule.ApplyToSingleElementNode(
                            element,
                            startNode,
                            this.octree.FindNodesWithin(
                                    startNode.LocationPoint,
                                    tolerance.Meters,
                                    startNode.Id
                                )
                                .Select(modelProposal.ApplyExistingProposal)
                                .Distinct()
                                .Where(node => node.Id != startNode.Id)
                                .ToList(),
                            Element1dSpatialHelper.FindElement1dsWithin(
                                this.element1ds,
                                modelProposal,
                                startNode.LocationPoint,
                                tolerance.Meters,
                                startNode.Id
                            ),
                            modelProposal,
                            tolerance
                        );
                    }
                    else if (startNodeVisited)
                    {
                        // Only start node is visited, apply rule to end node
                        rule.ApplyToSingleElementNode(
                            element,
                            endNode,
                            this.octree.FindNodesWithin(
                                    endNode.LocationPoint,
                                    tolerance.Meters,
                                    endNode.Id
                                )
                                .Select(modelProposal.ApplyExistingProposal)
                                .Distinct()
                                .Where(node => node.Id != endNode.Id)
                                .ToList(),
                            Element1dSpatialHelper.FindElement1dsWithin(
                                this.element1ds,
                                modelProposal,
                                endNode.LocationPoint,
                                tolerance.Meters,
                                endNode.Id
                            ),
                            modelProposal,
                            tolerance
                        );
                    }
                    else
                    {
                        List<Node> nearbyStartNodes = this
                            .octree.FindNodesWithin(
                                startNode.LocationPoint,
                                tolerance.Meters,
                                startNode.Id
                            )
                            .Select(modelProposal.ApplyExistingProposal)
                            .Distinct()
                            .Where(node => node.Id != startNode.Id)
                            .ToList();

                        List<Element1d> nearbyStartElements =
                            Element1dSpatialHelper.FindElement1dsWithin(
                                this.element1ds,
                                modelProposal,
                                startNode.LocationPoint,
                                tolerance.Meters,
                                startNode.Id
                            );
                        List<Node> nearbyEndNodes = this
                            .octree.FindNodesWithin(
                                endNode.LocationPoint,
                                tolerance.Meters,
                                endNode.Id
                            )
                            .Select(modelProposal.ApplyExistingProposal)
                            .Where(node => node.Id != endNode.Id)
                            .Distinct()
                            .ToList();

                        List<Element1d> nearbyEndElements =
                            Element1dSpatialHelper.FindElement1dsWithin(
                                this.element1ds,
                                modelProposal,
                                endNode.LocationPoint,
                                tolerance.Meters,
                                endNode.Id
                            );

                        rule.ApplyToBothElementNodes(
                            element,
                            startNode,
                            endNode,
                            nearbyStartNodes,
                            nearbyStartElements,
                            nearbyEndNodes,
                            nearbyEndElements,
                            modelProposal,
                            tolerance
                        );
                    }
                }
            }
        }
        return modelProposal.Build();
    }
}
