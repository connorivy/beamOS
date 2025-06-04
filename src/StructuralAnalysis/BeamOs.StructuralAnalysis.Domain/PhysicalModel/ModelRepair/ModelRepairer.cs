using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair.Rules;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public class ModelRepairer
{
    private readonly IList<Node> nodes;
    private readonly IList<Element1d> element1ds;
    private static readonly List<IModelRepairRule> rules =
    [
        new AlignBeamsIntoPlaneOfColumns(),
        new ExtendElement1dsInPlaneToNodeRule(),
        new ExtendCoplanarElement1dsToJoinNodes(),
        new ExtendColumnToMeetBeamRule(),
        new Element1dExtendOrShortenRule(),
        new NodeMergeRule(),
        new NodeSnapToElement1dRule(),
    ];
    private readonly Octree octree;
    private readonly ModelRepairOperationParameters modelRepairOperationParameters;

    public ModelRepairer(
        IList<Node> nodes,
        IList<Element1d> element1ds,
        Octree octree,
        ModelRepairOperationParameters modelRepairOperationParameters
    )
    {
        this.nodes = nodes;
        this.element1ds = element1ds;
        this.octree = octree;
        this.modelRepairOperationParameters = modelRepairOperationParameters;
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
            this.octree,
            this.modelRepairOperationParameters
        );

        for (int i = 0; i < 3; i++)
        {
            foreach (var rule in rules)
            {
                rule.Apply(
                    modelProposal,
                    this.modelRepairOperationParameters.GetToleranceForRule(rule) * (i + 1) / 3.0
                );
            }
        }
        return modelProposal.Build();
    }
}
