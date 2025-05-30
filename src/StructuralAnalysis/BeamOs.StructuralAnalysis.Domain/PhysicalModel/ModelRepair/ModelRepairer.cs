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
        new AlignBeamsIntoPlaneOfColumns(),
        new ExtendColumnToMeetBeamRule(),
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
            this.element1ds,
            this.octree
        );

        for (int i = 0; i < 3; i++)
        {
            var tolerance = this.tolerance * (i + 1) / 3.0;
            foreach (IModelRepairRule rule in rules)
            {
                rule.Apply(modelProposal, tolerance);
            }
        }
        return modelProposal.Build();
    }
}
